using Godot;
namespace Dust.Tools;

[Tool]
public partial class Dock : Panel
{
	public EditorPlugin Plugin { get; set; }

	[Export] private Label label;
	[Export] private ItemList itemsList;
	[Export] private UI_Vector3I start;
	[Export] private UI_Vector3I end;

	private Grid selectedGrid;
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		itemsList = GetNode<ItemList>("ItemList");
		start = GetNode<UI_Vector3I>("Start");
		end = GetNode<UI_Vector3I>("End");

		start.OnChange += v => selectedGrid?.SetBounds(new Bounds3I(start.Vector, end.Vector - start.Vector));
		end.OnChange += v => selectedGrid?.SetBounds(new Bounds3I(start.Vector, end.Vector - start.Vector));
	}

	public override void _Process(double delta)
	{
		if (Plugin is null || label is null || itemsList is null)
			return;

		var nodes = Plugin.GetEditorInterface().GetSelection().GetSelectedNodes();
		if (nodes is not null)
		{
			foreach (var node in nodes)
			{
				if (node is Grid grid)
				{
					selectedGrid = grid;
					break;
				}
			}
		}

		if (selectedGrid != null)
		{
			label.Text = $"Grid: {selectedGrid.Name}";
			var bounds = selectedGrid.GetBounds();

			start.Vector = bounds.Position;
			end.Vector = bounds.End;
		}
		else
		{
			label.Text = "Select a grid";
		}
	}
}
