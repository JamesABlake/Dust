using Godot;

namespace Dust;

public partial class CameraController : Node3D
{
	[Export] private Node3D Boom = null!;
	[Export] private Camera3D Camera = null!;


	[Export(PropertyHint.Range, "0.1, 10")] private float MoveSpeedMod = 1;
	[Export(PropertyHint.Range, "0.01, 10")] private float RotationSpeedMod = 1;
	[Export(PropertyHint.Range, "0.1, 10")] private float ZoomSpeedMod = 1;

	[Export(PropertyHint.Range, "1, 10")] private float MinZoom = 1;
	[Export(PropertyHint.Range, "10, 100")] private float MaxZoom = 50;

	private float _zoomPercent = 0.1f;

	public override void _Ready() => Camera.Position = new Vector3(0, 0, (Mathf.Pow(_zoomPercent, 2) * (MaxZoom - MinZoom)) + MinZoom);

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float moveSpeed = (float)delta * MoveSpeedMod;
		float rotationSpeed = (float)delta * RotationSpeedMod;
		float zoomSpeed = (float)delta * ZoomSpeedMod;

		if (Input.IsActionPressed("Forward"))
			TranslateObjectLocal(Vector3.Forward * moveSpeed);

		if (Input.IsActionPressed("Backward"))
			TranslateObjectLocal(Vector3.Back * moveSpeed);

		if (Input.IsActionPressed("Left"))
			TranslateObjectLocal(Vector3.Left * moveSpeed);

		if (Input.IsActionPressed("Right"))
			TranslateObjectLocal(Vector3.Right * moveSpeed);

		if (Input.IsActionPressed("Up"))
			TranslateObjectLocal(Vector3.Up * moveSpeed);

		if (Input.IsActionPressed("Down"))
			TranslateObjectLocal(Vector3.Down * moveSpeed);

		if (Input.IsMouseButtonPressed(MouseButton.Right))
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
			Boom.Rotation = new Vector3(Mathf.Clamp(Boom.Rotation.X + (Input.GetLastMouseVelocity().Y * -rotationSpeed), -Mathf.Pi / 2, 0), 0, 0);
			RotateY(Input.GetLastMouseVelocity().X * -rotationSpeed);
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (Input.IsActionJustPressed("ScrollUp"))
		{
			_zoomPercent = Mathf.Clamp(_zoomPercent - zoomSpeed, 0, 1);
			Camera.Position = new Vector3(0, 0, (Mathf.Pow(_zoomPercent, 2) * (MaxZoom - MinZoom)) + MinZoom);
		}

		if (Input.IsActionJustPressed("ScrollDown"))
		{
			_zoomPercent = Mathf.Clamp(_zoomPercent + zoomSpeed, 0, 1);
			Camera.Position = new Vector3(0, 0, (Mathf.Pow(_zoomPercent, 2) * (MaxZoom - MinZoom)) + MinZoom);
		}
	}
}
