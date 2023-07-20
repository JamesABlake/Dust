using Dust.Grids;
using System.Diagnostics;

namespace Dust.Tools;

public abstract partial class GridTool : Tool
{
	protected GridEditor GridEditor = null!;
	protected Grid3D Grid3D => GridEditor.TargetGrid;

	public void Setup(GridEditor gridEditor) => GridEditor = gridEditor;

	protected override void OnActivate()
	{
		Debug.Assert(GridEditor is not null, $"{nameof(GridEditor)} must be set before activating.");
	}
}