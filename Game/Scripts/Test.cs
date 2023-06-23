using Godot;
namespace Dust;
public partial class Test : Node3D
{
	private Vector2 _mousePosition;

	public override void _PhysicsProcess(double delta)
	{
		var camera3D = GetNode<Camera3D>("Camera3D");
		var from = camera3D.ProjectRayOrigin(_mousePosition);
		var to = from + (camera3D.ProjectRayNormal(_mousePosition) * 100);

		var space = GetWorld3D().DirectSpaceState;
		var ray = PhysicsRayQueryParameters3D.Create(from, to);
		ray.CollideWithBodies = true;
		var result = space.IntersectRay(ray);

		if (result.Count > 0)
			GD.Print($"Hit at {result["position"]}");
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
			_mousePosition = eventMouseMotion.Position;
	}
}
