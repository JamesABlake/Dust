using Godot;
using System.Collections.Generic;

namespace Dust.Grids;
public partial class Cell : Resource
{
	public Edge? Floor;
	public readonly List<Edge> Walls = new();
}
