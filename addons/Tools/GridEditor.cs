#if TOOLS
using Godot;
namespace Dust.Tools;

[Tool]
public partial class GridEditor : EditorPlugin
{
	private Dock _dock;
	private GridEditorGizmo _gizmo;
	public override void _EnterTree()
	{
		_gizmo = (GridEditorGizmo)ResourceLoader.Load<CSharpScript>("addons/Tools/GridEditorGizmo.cs").New();
		_dock = (Dock)GD.Load<PackedScene>("addons/Tools/Dock.tscn").Instantiate();
		_dock.Plugin = this;

		AddNode3DGizmoPlugin(_gizmo);
		AddControlToDock(DockSlot.RightBl, _dock);
	}

	public override void _ExitTree()
	{
		RemoveNode3DGizmoPlugin(_gizmo);
		RemoveControlFromDocks(_dock);

		_dock.Free();
	}
}
#endif
