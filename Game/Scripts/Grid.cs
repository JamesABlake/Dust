
using Godot;
namespace Dust;

[Tool]
public partial class Grid : Node3D, IGrid
{
	private Bounds3I bounds = new(-Vector3I.One * 2, Vector3I.One * 4);
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public bool HasCell(Vector3I position) => throw new System.NotImplementedException();
	public IGrid.ICell GetCell(Vector3I position) => throw new System.NotImplementedException();
	public bool TryGetCell(Vector3I position, out IGrid.ICell cell) => throw new System.NotImplementedException();
	public Bounds3I GetBounds() => bounds;
	public void SetBounds(Bounds3I bounds) => this.bounds = bounds;
}
