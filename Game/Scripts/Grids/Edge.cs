using Godot;

namespace Dust.Grids;

[GlobalClass]
public partial class Edge : Node3D
{
	[Export] public Direction DefaultDirection { get; set; }
	[Export] public Direction PlacementDirectionsAllowed { get; set; }
	[Export] public bool CanPass { get; set; }

	public void Setup(Vector3 globalPosition, Direction direction)
	{
		Position = globalPosition;
		RotationDegrees = direction switch
		{
			Direction.Forward => Vector3.Zero,
			Direction.Right => new Vector3(0, 90, 0),
			Direction.Backward => new Vector3(0, 180, 0),
			Direction.Left => new Vector3(0, 270, 0),
			_ => Vector3.Zero
		};
	}
}
