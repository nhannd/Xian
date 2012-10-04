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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

// ... (other using namespace statements)

namespace MyPlugin.Miscellaneous
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarMyStudyBrowserTool", "Activate")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuMyStudyBrowserTool", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipMyStudyBrowserTool")]
	[IconSet("activate", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes)
	[ExtensionOf(typeof (StudyBrowserToolExtensionPoint))]
	public class MyStudyBrowserTool : StudyBrowserTool
	{
		public MyStudyBrowserTool() {}

		public void Activate()
		{
			StudyItem item = this.Context.SelectedStudy;

			// Do something with the selected study
			base.Context.DesktopWindow.ShowMessageBox(item.StudyInstanceUid, MessageBoxActions.Ok);
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			// Executed if the selected server changed
		}
	}
}