using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using System;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("apply", "folderexplorer-folders-contextmenu/Refresh", "Refresh")]
	[ButtonAction("apply", "folderexplorer-folders-toolbar/Refresh", "Refresh")]
	[Tooltip("apply", "Refresh Folders")]
	[IconSet("apply", IconScheme.Colour, "Icons.RefreshToolSmall.png", "Icons.RefreshToolMedium.png", "Icons.RefreshToolLarge.png")]
	public abstract class RefreshTool<TWorkflowFolderToolContext> : Tool<TWorkflowFolderToolContext>
		where TWorkflowFolderToolContext : IWorkflowFolderToolContext
	{
		public void Refresh()
		{
			this.Context.InvalidateFolders();
		}
	}
}