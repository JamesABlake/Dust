/*
using Godot;
using System;
using System.Text.RegularExpressions;
namespace Dust;

[Tool]
public partial class NumberLine : LineEdit
{
	public Action<int> OnChange;
	public int Value
	{
		get => int.Parse(Text);
		set
		{
			Text = value.ToString();
			OnChange?.Invoke(value);
		}
	}
	public override void _Ready()
	{
		TextSubmitted += NumberLine_TextChanged;
		Text = "0";
	}

	private void NumberLine_TextChanged(string newText)
	{
		GD.Print($"Checking {newText}");
		if (Regex.IsMatch(newText, @"^[0-9]*$"))
		{
			Text = newText;
			GD.Print($"Changed to {newText}");
		}
	}
}
*/