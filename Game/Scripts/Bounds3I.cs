using Godot;

namespace Dust;
public record struct Bounds3I
{
	private Vector3I _position;
	private Vector3I _size;

	public Vector3I Position { readonly get => _position; set => _position = value; }
	public Vector3I Size { readonly get => _size; set => _size = value; }
	public Vector3I End { readonly get => _position + _size; set => _size = _position + _size - value; }

	#region Constructors
	public Bounds3I()
	{
		_position = Vector3I.Zero;
		_size = Vector3I.Zero;
	}
	public Bounds3I(Bounds3I from)
	{
		_position = from._position;
		_size = from._size;
	}
	public Bounds3I(Vector3I position, Vector3I size)
	{
		_position = position;
		_size = size;
	}
	#endregion

	#region Methods
	public Bounds3I Abs()
	{
		Vector3I lowest = MathV.Min(Position, End);
		Vector3I highest = MathV.Max(Position, End);

		Position = lowest;
		End = highest;
		return this;
	}
	public readonly bool Encloses(Bounds3I bounds) => Position <= bounds.Position && bounds.End <= End;
	public Bounds3I Expand(Vector3I to)
	{
		Vector3I lowest = MathV.Min(Position, to);
		Vector3I highest = MathV.Max(End, to);
		Position = lowest;
		End = highest;
		return this;
	}
	public readonly Vector3I GetCenter() => Position + (Size / 2);
	public readonly bool HasPoint(Vector3I point) => point >= Position && point <= End;
	#endregion
}
