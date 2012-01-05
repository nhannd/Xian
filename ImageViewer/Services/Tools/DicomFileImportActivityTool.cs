#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuDicomFileImportActivity", "Activate")]
	//[IconSet("activate", "", "Icons.DicomFileImportActivityMedium.png", "Icons.DicomFileImportActivityLarge.png")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	[ViewerActionPermission("activate", ImageViewer.Services.AuthorityTokens.Study.Import)]

	public class DicomFileImportActivityTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		public DicomFileImportActivityTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public void Activate()
		{
			try
			{
				LocalDataStoreActivityMonitorComponentManager.ShowImportComponent(this.Context.DesktopWindow);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToLaunchImportActivityComponent, this.Context.DesktopWindow);
			}
		}
	}
}
