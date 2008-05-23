using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
	[ButtonAction("search", "folderexplorer-folders-toolbar/Search", "Search")]
	[ButtonAction("search", "folders-toolbar/Search")]
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

	[MenuAction("search", "homepage-contextmenu/Search", "Search")]
	[ButtonAction("search", "homepage-toolbar/Search", "Search")]
	[Tooltip("search", "Search")]
	[IconSet("search", IconScheme.Colour, "ClearCanvas.Ris.Client.Icons.SearchToolSmall.png", "ClearCanvas.Ris.Client.Icons.SearchToolMedium.png", "ClearCanvas.Ris.Client.Icons.SearchToolLarge.png")]
	[ExtensionOf(typeof(HomePageToolExtensionPoint))]
	public class HomePageSearchTool : Tool<IHomePageToolContext>
	{
		public void Search()
		{
			SearchComponent.Launch(this.Context.DesktopWindow);
		}
	}
}
