using Godot;
using System.Threading.Tasks;

namespace Dust;
public static class UIHelpers
{
	public static async Task<bool> ConfirmationDialog(this Node node, string dialogText)
	{
		var dialog = new ConfirmationDialog
		{
			DialogText = dialogText
		};
		TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

		dialog.Confirmed += () => result.SetResult(true);
		dialog.Canceled += () => result.SetResult(false);

		node.GetTree().Root.CallDeferred("add_child", dialog);
		dialog.Show();

		return await result.Task;
	}
}
