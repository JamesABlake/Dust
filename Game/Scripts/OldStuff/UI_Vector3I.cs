/*
using Godot;
using System;

namespace Dust;

[Tool]
public partial class UI_Vector3I : Control
{
	public Action<Vector3I> OnChange;
	public Vector3I Vector
	{
		get => new(X.Value, Y.Value, Z.Value);
		set
		{
			X.Value = value.X;
			Y.Value = value.Y;
			Z.Value = value.Z;
			OnChange(value);
		}
	}

	[Export] private NumberLine X;
	[Export] private NumberLine Y;
	[Export] private NumberLine Z;

	public override void _Ready()
	{
		X = GetNode<NumberLine>("X");
		Y = GetNode<NumberLine>("Y");
		Z = GetNode<NumberLine>("Z");

		X.OnChange += _ => OnChange(Vector);
		Y.OnChange += _ => OnChange(Vector);
		Z.OnChange += _ => OnChange(Vector);
	}
}
*/