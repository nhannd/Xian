using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("apply", "folderexplorer-folders-contextmenu/Refresh", "Refresh")]
	[ButtonAction("apply", "folderexplorer-folders-toolbar/Refresh", "Refresh")]
	[Tooltip("apply", "Refresh Folder")]
	[IconSet("apply", IconScheme.Colour, "Icons.RefreshToolSmall.png", "Icons.RefreshToolMedium.png", "Icons.RefreshToolLarge.png")]
	public abstract class RefreshTool<TWorkflowFolderToolContext> : Tool<TWorkflowFolderToolContext>
		where TWorkflowFolderToolContext : IWorkflowFolderToolContext
	{
		public void Refresh()
		{
			this.Context.SelectedFolder.Refresh();
		}
	}

	[MenuAction("apply", "folderexplorer-group-contextmenu/Refresh", "Refresh")]
	[ButtonAction("apply", "folderexplorer-group-toolbar/Refresh", "Refresh")]
	[Tooltip("apply", "Refresh")]
	[IconSet("apply", IconScheme.Colour, "Icons.RefreshToolSmall.png", "Icons.RefreshToolMedium.png", "Icons.RefreshToolLarge.png")]
	[ExtensionOf(typeof(FolderExplorerGroupToolExtensionPoint))]
	public class HomePageRefreshTool : Tool<IFolderExplorerGroupToolContext>
	{
		public void Refresh()
		{
			this.Context.SelectedFolder.Refresh();
		}
	}
}