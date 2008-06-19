using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	//[ButtonAction("search", "folderexplorer-folders-toolbar/Search", "Search")]
	//[ButtonAction("search", "folders-toolbar/Search")]
	[Tooltip("search", "Search")]
	[IconSet("search", IconScheme.Colour, "ClearCanvas.Ris.Client.Icons.SearchToolSmall.png", "ClearCanvas.Ris.Client.Icons.SearchToolMedium.png", "ClearCanvas.Ris.Client.Icons.SearchToolLarge.png")]
	public abstract class SearchTool<TWorkflowFolderToolContext> : Tool<TWorkflowFolderToolContext>
		where TWorkflowFolderToolContext : IWorkflowFolderToolContext
	{
		public void Search()
		{
			SearchComponent.Launch(this.Context.DesktopWindow);
		}
	}
}
