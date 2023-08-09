using Dust.Grids;
using Godot;

namespace Dust.Tools;

public partial class FloorDesigner : GridTool
{
	[Export] private Button TileButton = null!;
	[Export] private Button AngleButton = null!;
	[Export] private Button ArcButton = null!;

	[Export] private Material IntersectMaterial = null!;

	[Export] private MultiMeshInstance3D PointMultiMesh = null!;

	[Export] private MeshInstance3D TileMesh = null!;
	[Export] private MeshInstance3D AngleMesh = null!;
	// Has to be generated, can't just be scaled to fit.
	private MeshInstance3D? ArcMesh;

	private Vector3 lastMousePosition;
	private (Vector3? A, Vector3? B, Vector3? C) points;

	private Mode _mode;
	private enum Mode { None, Tile, Angled, Arc }

	public override void _Ready()
	{
		TileButton.Pressed += () => SetMode(Mode.Tile, TileButton);
		AngleButton.Pressed += () => SetMode(Mode.Angled, AngleButton);
		ArcButton.Pressed += () => SetMode(Mode.Arc, ArcButton);
	}

	protected override void OnMouseMove(Ray mouseRay)
	{
		if (!Grid3D.TryGetIntersection(mouseRay, GridEditor.TargetHeight + 0.05f, out Vector3 hit))
			return;

		lastMousePosition = hit;



		_ = _mode switch
		{
			Mode.Tile => UpdateTileVisuals(hit),
			Mode.Angled => UpdateAngledVisuals(hit),
			Mode.Arc => UpdateArcVisuals(hit),
			_ => false
		};
	}
	private bool UpdateTileVisuals(Vector3 position)
	{
		Vector3 snapped = Grid3D.SnapToGrid(position);
		Vector3I gridspace = Grid3D.ToGridspace(position);

		TileMesh.Visible = true;
		TileMesh.GlobalPosition = snapped + new Vector3(0, -0.5f, 0);
		TileMesh.MaterialOverride = (Grid3D.TryGetCell(gridspace, out var cell) && cell.Floor is not null) ? IntersectMaterial : null;

		return true;
	}
	private bool UpdateAngledVisuals(Vector3 position)
	{
		MultiMesh mesh = PointMultiMesh.Multimesh;
		mesh.InstanceCount = 1 + (points.A is null ? 0 : 1) + (points.B is null ? 0 : 1);
		if (points.A is not null)
		{
			mesh.SetInstanceTransform(1, Transform3D.Identity.Translated(points.A.Value));
		}

		if (points.B is not null)
		{
			mesh.SetInstanceTransform(2, Transform3D.Identity.Translated(points.B.Value));
		}

		mesh.SetInstanceTransform(0, Transform3D.Identity.Translated(Grid3D.SnapToVert(position)));
		return true;
	}
	private bool UpdateArcVisuals(Vector3 position)
	{
		return true;
	}
	private void Reset()
	{
		TileMesh.Visible = false;
		PointMultiMesh.Multimesh.InstanceCount = 0;

		points.A = null;
		points.B = null;
	}

	protected override void OnMouseButtonDown(MouseButton button)
	{
		switch (button, _mode)
		{
			case (_, Mode.None):
				return;

			case (MouseButton.Left, Mode.Tile):
				HandleTilePlacement();
				break;

			case (MouseButton.Left, Mode.Angled):
				HandleAngledPlacement();
				break;
		}
	}
	private void HandleTilePlacement()
	{
		Vector3I gridspace = Grid3D.ToGridspace(lastMousePosition);
		Grid3D.GetOrAddCell(gridspace, out Cell cell);
		if (cell.Floor is null)
		{
			MeshInstance3D mesh = (MeshInstance3D)TileMesh.Duplicate();

			Edge floor = new();
			Grid3D.AddChild(floor);
			floor.GlobalPosition = Grid3D.ToGlobal(gridspace);
			floor.AddChild(mesh);
			mesh.Position = new Vector3(0, -0.5f, 0);

			cell.Floor = floor;
		}
	}
	private void HandleAngledPlacement()
	{
		if (points.A is null)
		{
			points.A = Grid3D.SnapToVert(lastMousePosition);
			return;
		}

		if (points.B is null)
		{
			points.B = Grid3D.SnapToVert(lastMousePosition);
			return;
		}

		Vector3 pointA = points.A.Value;
		Vector3 pointB = points.B.Value;

		Vector3I vertA = Grid3D.ToVertspace(pointA);
		Vector3I vertB = Grid3D.ToVertspace(pointB);
		Vector3I vertC = Grid3D.ToVertspace(lastMousePosition);

		// They don't fall on the same grid line, so they form the angled part
		if (vertA.X != vertB.X && vertA.Z != vertB.Z)
		{
			if ((vertC.X != vertA.X || vertC.Z != vertB.Z) && (vertC.X != vertB.X || vertC.Y != vertA.Y))
				return;

			MeshInstance3D mesh = (MeshInstance3D)AngleMesh.Duplicate();

			Grid3D.AddChild(mesh);

			mesh.GlobalPosition = (pointA + pointB) / 2;
			float rotation = GetAngledRotation((pointA + pointB) / 2, lastMousePosition);
			mesh.RotateY(rotation);
			mesh.Scale = new Vector3(Mathf.Abs(pointA.X - pointB.X), 1, Mathf.Abs(pointA.Z - pointB.Z)).Rotated(Vector3.Up, rotation).Abs();
			mesh.Visible = true;

			Reset();
		}
		else
			Reset();
	}

	private float GetAngledRotation(Vector3 center, Vector3 anglePosition)
	{
		// Godot uses negative Z for north, positive X for East
		return Mathf.Pi + (anglePosition - center) switch
		{
			{ X: > 0, Y: _, Z: < 0 } => 0,
			{ X: < 0, Y: _, Z: < 0 } => Mathf.Pi / 2,
			{ X: < 0, Y: _, Z: > 0 } => Mathf.Pi,
			{ X: > 0, Y: _, Z: > 0 } => Mathf.Pi / 2 * 3,
			_ => throw new System.ArgumentException($"{nameof(center)} and {nameof(anglePosition)} should not overlap")
		};
	}

	private void SetMode(Mode mode, Button button)
	{
		if (mode == _mode)
		{
			button.ReleaseFocus();
			_mode = Mode.None;
			Reset();
			return;
		}

		TileMesh.Visible = false;
		AngleMesh.Visible = false;
		if (ArcMesh != null)
		{
			ArcMesh.Visible = false;
		}

		_mode = mode;
		Reset();
	}
}
