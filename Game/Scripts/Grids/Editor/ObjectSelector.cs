using Godot;
namespace Dust.Grids;

public partial class ObjectSelector : Control
{
	public PackedScene? SelectedScene => currentIndex.HasValue ? Palette[currentIndex.Value] : null;

	[Export] private PackedScene[] Palette = null!;
	[Export] private Label Label = null!;
	[Export] private Container ButtonContainer = null!;
	[Export] private PackedScene ButtonPrefab = null!;

	private int? currentIndex;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < Palette.Length; i++)
		{
			int lambdaPasser = i;
			var button = ButtonPrefab.Instantiate<Button>();
			string path = Palette[i].ResourcePath;

			button.Text = path[(path.LastIndexOf('/') + 1)..][..(path.Length - path.IndexOf('.') - 1)];

			button.Pressed += () => Button_Pressed(button, lambdaPasser);

			ButtonContainer.CallDeferred("add_child", button);
		}
	}

	private void Button_Pressed(Button source, int index)
	{
		if (currentIndex != index)
		{
			currentIndex = index;
			Label.Text = source.Text;
		}
		else
		{
			currentIndex = null;
			Label.Text = "Select an item";
			source.ReleaseFocus();
		}
	}
}