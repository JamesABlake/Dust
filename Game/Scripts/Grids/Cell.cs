using Godot;
using System.Collections.Generic;

namespace Dust.Grids;
public partial class Cell : Resource
{
	private readonly Dictionary<Direction, Edge> _edges = new();

	public bool TryAddEdge(Direction direction, Edge edge) => edge.PlacementDirectionsAllowed.HasFlag(direction) && _edges.TryAdd(direction, edge);
}
