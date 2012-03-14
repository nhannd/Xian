#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuLocalDataStoreReindex", "Activate")]
	//[IconSet("activate", "", "Icons.LocalDataStoreReindexMedium.png", "Icons.LocalDataStoreReindexLarge.png")]
	[ViewerActionPermission("activate", AuthorityTokens.Administration.ReindexLocalDataStore)]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class LocalDataStoreReindexTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		public LocalDataStoreReindexTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public void Activate()
		{
			LocalDataStoreActivityMonitorComponentManager.ShowReindexComponent(this.Context.DesktopWindow);
		}
	}
}
