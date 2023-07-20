using Dust.Tools;
using Godot;

namespace Dust.Grids;

public partial class GridEditor : Node
{
	public int TargetHeight { get; private set; }

	[Export] private Grid3D Grid3D = null!;
	[Export] private FloorDesigner FloorDesigner = null!;

	private Tool? SelectedTool;

	public Grid3D TargetGrid => Grid3D;

	public override void _Ready()
	{
		FloorDesigner.Setup(this);
		SelectedTool = FloorDesigner;
	}

	public override void _Process(double delta)
	{
		Camera3D camera = GetViewport().GetCamera3D();

		Vector3 cameraBasePosition = camera.GetParentNode3D().GetParentNode3D().GlobalPosition;
		TargetHeight = Grid3D.GetHeightAt(cameraBasePosition);
		Grid3D.VisualizeGridAtLocalHeight(TargetHeight);

		Ray ray = new Ray(camera.GlobalPosition, camera.ProjectRayNormal(GetViewport().GetMousePosition()));
		FloorDesigner.MouseMove(ray);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (SelectedTool is null)
			return;

		switch (@event)
		{
			case InputEventMouseButton mouse:
				if (mouse.IsPressed())
					SelectedTool.MouseButtonDown(mouse.ButtonIndex);
				else if (mouse.IsReleased())
					SelectedTool.MouseButtonUp(mouse.ButtonIndex);
				break;
			case InputEventKey key:
				if (key.IsPressed())
					SelectedTool.KeyDown(key.PhysicalKeycode);
				else if (key.IsReleased())
					SelectedTool.KeyUp(key.PhysicalKeycode);
				break;
		}
	}
}
