using Godot;
using System.Collections.Generic;
namespace Dust.Tools;

[Tool]
public partial class GridEditorGizmo : EditorNode3DGizmoPlugin
{
	public GridEditorGizmo()
	{
		CreateMaterial("main", new Color(1, 0, 0));
	}

	public override string _GetGizmoName() => "Grid Editor";
	public override bool _HasGizmo(Node3D forNode3D)
	{
		return forNode3D is Grid;
	}
	public override void _Redraw(EditorNode3DGizmo gizmo)
	{
		gizmo.Clear();

		var node = gizmo.GetNode3D();
		var grid = (node as Grid);
		var bounds = grid.GetBounds();

		var lines = new List<Vector3>();
		for (int x = bounds.Position.X; x <= bounds.End.X; x++)
		{
			lines.Add(new Vector3I(x, 0, bounds.Position.Z));
			lines.Add(new Vector3I(x, 0, bounds.End.Z));
		}
		for (int z = bounds.Position.Z; z <= bounds.End.Z; z++)
		{
			lines.Add(new Vector3I(bounds.Position.X, 0, z));
			lines.Add(new Vector3I(bounds.End.X, 0, z));
		}

		gizmo.AddLines(lines.ToArray(), GetMaterial("main", gizmo), false);
	}
}
