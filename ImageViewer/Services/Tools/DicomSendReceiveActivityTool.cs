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
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuDicomSendReceiveActivity", "Activate")]
	[ViewerActionPermission("activate", ImageViewer.Common.AuthorityTokens.Study.Send)]
    [ViewerActionPermission("activate", ImageViewer.Common.AuthorityTokens.Study.Retrieve)]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class DicomSendReceiveActivityTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		public DicomSendReceiveActivityTool()
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
				LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(this.Context.DesktopWindow);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToLaunchSendReceiveActivityComponent, this.Context.DesktopWindow);
			}
		}
	}
}
