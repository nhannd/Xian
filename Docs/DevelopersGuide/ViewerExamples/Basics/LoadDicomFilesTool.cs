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
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

// ... (other using namespace statements here)

namespace MyPlugin.Basics
{
	[ButtonAction("open", "global-toolbars/ToolbarStandard/OpenFiles", "Open")]
	[IconSet("open", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class LoadDicomFilesTool : ImageViewerTool
	{
		public void Open()
		{
			string[] filenames = new string[] {"C:\\File1.dcm", "C:\\File2.dcm"};

			this.LoadStudy(filenames);
		}

		public void LoadStudy(string[] filenames)
		{
			try
			{
				OpenStudyHelper.OpenFiles(filenames, ViewerLaunchSettings.WindowBehaviour);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}