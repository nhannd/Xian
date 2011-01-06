#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
				this.Context.SelectedFolderSystem.InvalidateFolders(true);
			}
		}
	}
}