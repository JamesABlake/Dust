using System;
namespace Dust;
using static Direction;

[Flags]
public enum Direction
{
	None = 0,
	Forward = 1,
	Backward = 2,
	Left = 4,
	Right = 8,
	Up = 16,
	Down = 32,

	Cardinal = Forward | Backward | Left | Right,
}

public static class DirectionExtensions
{
	public static Direction RotateLeft(this Direction direction)
	{
		return direction switch
		{
			Forward => Left,
			Left => Backward,
			Backward => Right,
			Right => Forward,
			Up => Up,
			Down => Down,
			_ => throw new NotSupportedException("Only pure translations are supported")
		};
	}

	public static Direction RotateRight(this Direction direction)
	{
		return direction switch
		{
			Forward => Right,
			Right => Backward,
			Backward => Left,
			Left => Forward,
			Up => Up,
			Down => Down,
			_ => throw new NotSupportedException("Only pure translations directions are supported")
		};
	}
}