using Godot;

namespace Dust.Grids;

public partial class GridEditor : Node
{
	[Export] private int Radius = 10;
	[Export] private ObjectSelector Selector = null!;
	[Export] private Grid3D Grid3D = null!;

	private MeshInstance3D _mouseMeshInstance = new();

	private Direction _placementDirection = Direction.Forward;

	public override void _Ready()
	{
		GenerateMouse();
	}

	public override void _Process(double delta)
	{
		int height = Mathf.FloorToInt(GetViewport().GetCamera3D().GetParentNode3D().GetParentNode3D().GlobalPosition.Y);
		Grid3D.VisualizeGridAtLocalHeight(height);

		UpdateMouseMesh();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton button && button.IsReleased() && button.ButtonIndex == MouseButton.Left && Selector.SelectedScene != null)
		{
			AddToGrid(Selector.SelectedScene);
		}

		if (@event is InputEventKey key && key.IsReleased() && Selector.SelectedScene != null)
		{
			switch (key.Keycode)
			{
				case Key.E:
					_placementDirection = _placementDirection.RotateLeft();
					break;
				case Key.Q:
					_placementDirection = _placementDirection.RotateRight();
					break;
			}

		}
	}


	private void GenerateMouse()
	{
		var immediate_mesh = new ImmediateMesh();

		_mouseMeshInstance.Mesh = immediate_mesh;
		_mouseMeshInstance.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

		var offset = new Vector3(-0.5f, -0.5f, -0.5f);

		immediate_mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				immediate_mesh.SurfaceAddVertex(new Vector3(i, j, 0) + offset);
				immediate_mesh.SurfaceAddVertex(new Vector3(i, j, 1) + offset);

				immediate_mesh.SurfaceAddVertex(new Vector3(i, 0, j) + offset);
				immediate_mesh.SurfaceAddVertex(new Vector3(i, 1, j) + offset);

				immediate_mesh.SurfaceAddVertex(new Vector3(0, i, j) + offset);
				immediate_mesh.SurfaceAddVertex(new Vector3(1, i, j) + offset);
			}
		}
		immediate_mesh.SurfaceEnd();

		_ = GetTree().Root.CallDeferred("add_child", _mouseMeshInstance);
	}

	private void UpdateMouseMesh()
	{
		if (TryGetMouseLocationOnGrid(out var position))
		{
			_mouseMeshInstance.GlobalPosition = Grid3D.ToGlobal(position);
			_mouseMeshInstance.GlobalRotation = Grid3D.GlobalRotation;
			_mouseMeshInstance.Visible = true;
		}
		else
		{
			_mouseMeshInstance.Visible = false;
		}
	}

	private void AddToGrid(PackedScene scene)
	{
		if (TryGetMouseLocationOnGrid(out var position))
		{
			Grid3D.GetOrAddCell(position, out Cell cell);

			var instance = scene.Instantiate<Node3D>();
			if (instance is Edge edge)
			{
				// Check if the current rotation is allowed by the current instance. If not, set it to the default one.
				var direction = edge.PlacementDirectionsAllowed & _placementDirection;
				if (direction == Direction.None)
					direction = edge.DefaultDirection;

				if (cell.TryAddEdge(direction, edge))
				{
					Grid3D.AddChild(edge);
					edge.Setup(position, direction);
				}
				else
				{
					edge.Free();
				}
			}
		}
	}

	private bool TryGetMouseLocationOnGrid(out Vector3I hitPosition)
	{
		Camera3D camera = GetViewport().GetCamera3D();
		Vector3 origin = camera.GlobalPosition;
		Vector3 direction = camera.ProjectRayNormal(GetViewport().GetMousePosition());
		int planeHeight = Mathf.FloorToInt(camera.GetParentNode3D().GetParentNode3D().GlobalPosition.Y);
		return Grid3D.TryGetRayIntersectionCell(origin, direction, planeHeight, out hitPosition);
	}
}
