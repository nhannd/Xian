using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using System;

namespace ClearCanvas.Ris.Client
{
	[ButtonAction("apply", "folderexplorer-folders-toolbar/Refresh All", "Refresh")]
	[MenuAction("apply", "folderexplorer-folders-contextmenu/Refresh All", "Refresh")]
	[Tooltip("apply", "Refresh Folders")]
	[IconSet("apply", IconScheme.Colour, "Icons.RefreshSmall.png", "Icons.RefreshMedium.png", "Icons.RefreshLarge.png")]
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