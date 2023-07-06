using Godot;

namespace Dust;
public interface IGrid
{
	bool HasCell(Vector3I position);
	ICell GetCell(Vector3I position);
	bool TryGetCell(Vector3I position, out ICell cell);
	Bounds3I GetBounds();
	void SetBounds(Bounds3I bounds);
	public interface ICell { }
}
