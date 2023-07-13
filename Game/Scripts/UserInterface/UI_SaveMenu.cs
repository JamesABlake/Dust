using Dust;
using Godot;

public partial class UI_SaveMenu : Panel
{
	[Export] private Button Save = null!;
	[Export] private LineEdit Input = null!;
	[Export] private Button Accept = null!;
	[Export] private Button Cancel = null!;

	public override void _Ready()
	{
		Save.Pressed += Open;
		Input.TextSubmitted += _ => Input_Accept();
		Accept.Pressed += Input_Accept;
		Cancel.Pressed += Input_Cancel;
	}

	private void Open()
	{
		Show();
	}

	private async void Input_Accept()
	{
		string path = $"user://{Input.Text}.txt";
		if (FileAccess.FileExists(path) && !await UIHelpers.ConfirmationDialog(this, $"File '{path} already exists. Overwrite?"))
		{
			return;
		}

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString("Saved!");
		Hide();
	}

	private void Input_Cancel()
	{
		Hide();
	}


}
