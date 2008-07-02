using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using System;

namespace ClearCanvas.Ris.Client
{
	[ButtonAction("apply", "folderexplorergroup-toolbar/Refresh", "Refresh")]
	[Tooltip("apply", "Refresh Folders")]
	[IconSet("apply", IconScheme.Colour, "Icons.RefreshToolSmall.png", "Icons.RefreshToolMedium.png", "Icons.RefreshToolLarge.png")]
	[ExtensionOf(typeof(FolderExplorerGroupToolExtensionPoint))]
	public class FolderRefreshTool : Tool<IFolderExplorerGroupToolContext>
	{
		public void Refresh()
		{
			if(this.Context.SelectedFolderSystem != null)
			{
				this.Context.SelectedFolderSystem.InvalidateFolders();
			}
		}
	}
}