using Godot;
using Godot.Collections;
using System.Linq;

namespace Dust.Grids;
public partial class CellPart : Node3D
{
	[Export] public Dictionary<Vector3I, Direction> Edges { get; protected set; } = new();

	[Export] public Direction PlacementDirectionsAllowed { get; protected set; }
	public Bounds3I Bounds { get; protected set; }

	public CellPart()
	{
		Bounds = new Bounds3I(Vector3I.Zero, MathV.Max(Edges.Keys.ToArray()));
	}
}
