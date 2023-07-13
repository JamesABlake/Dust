using Godot;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dust.Grids;

[GlobalClass]
public partial class Grid3D : Node3D
{
	private Bounds3I _boundingBox;
	private MeshInstance3D? _gridVisualization;
	private readonly Dictionary<Vector3I, Cell> _grid = new();
	private const int Radius = 10;
	private readonly Vector3 VisualOffset = new(-0.5f, -0.5f, -0.5f);

	/// <summary>
	/// Gets or adds a cell at a position.
	/// </summary>
	/// <param name="position">The position to add a cell to</param>
	/// <param name="cell">The cell added or retrieved</param>
	/// <returns>True if a new cell was created, false if one already existed</returns>
	public bool GetOrAddCell(Vector3I position, out Cell cell)
	{
		if (_grid.TryGetValue(position, out cell!))
			return false;

		cell = new Cell();
		_grid.Add(position, cell);
		_boundingBox.Expand(position);
		return true;
	}

	public bool RemoveCell(Vector3I position)
	{
		bool modified = _grid.Remove(position);
		if (modified)
		{
			_boundingBox = new Bounds3I();
			foreach (var cellPosition in _grid.Keys)
				_boundingBox.Expand(cellPosition);
		}
		return modified;
	}
	public bool HasCell(Vector3I position) => _grid.ContainsKey(position);
	public Cell GetCell(Vector3I position) => _grid[position];
	public bool TryGetCell(Vector3I position, [MaybeNullWhen(false)] out Cell cell) => _grid.TryGetValue(position, out cell);
	public Bounds3I GetBounds() => _boundingBox;
	public void VisualizeGridAtLocalHeight(int height)
	{
		// If the grid is null, generate it
		if (_gridVisualization is null)
		{

			var immediate_mesh = new ImmediateMesh();

			_gridVisualization = new()
			{
				Mesh = immediate_mesh,
				CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
			};

			immediate_mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
			for (int line = -Radius + 1; line <= Radius; line++)
			{
				// Lines perpendicular to the X axis
				immediate_mesh.SurfaceAddVertex(new Vector3(line, 0, -Radius + 1) + VisualOffset);
				immediate_mesh.SurfaceAddVertex(new Vector3(line, 0, Radius) + VisualOffset);

				// Lines perpendicular to the Z axis
				immediate_mesh.SurfaceAddVertex(new Vector3(-Radius + 1, 0, line) + VisualOffset);
				immediate_mesh.SurfaceAddVertex(new Vector3(Radius, 0, line) + VisualOffset);
			}
			immediate_mesh.SurfaceEnd();

			_ = CallDeferred("add_child", _gridVisualization);
		}

		_gridVisualization.Position = _gridVisualization.Position.With(null, height, null);
	}

	public bool TryGetRayIntersectionCell(Vector3 origin, Vector3 direction, int planeHeight, out Vector3I hit)
	{
		Plane plane = new(Vector3.Up, planeHeight - 0.5f);
		Vector3? planeHit = plane.IntersectsRay(ToLocal(origin), ToLocal(direction));
		if (planeHit.HasValue)
		{
			hit = MathV.RoundToInt(planeHit.Value);
			hit.Y = planeHeight;
			return true;
		}

		hit = Vector3I.Zero;
		return false;
	}
}
