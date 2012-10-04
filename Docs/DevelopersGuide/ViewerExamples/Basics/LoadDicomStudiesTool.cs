#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
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
	[ButtonAction("open", "global-toolbars/ToolbarStandard/OpenStudies", "Open")]
	[IconSet("open", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class LoadDicomStudiesTool : ImageViewerTool
	{
		public void Open()
		{
			string[] studyInstanceUids = {"1.2.840.1234567890.1.1", "1.2.840.1234567890.1.2", "1.2.840.1234567890.1.3"};

			this.LoadStudies(studyInstanceUids);
		}

		public void LoadStudies(IEnumerable<string> studyInstanceUids)
		{
			try
			{
				OpenStudyHelper helper = new OpenStudyHelper();
				helper.WindowBehaviour = ViewerLaunchSettings.WindowBehaviour;
				foreach (string studyInstanceUid in studyInstanceUids)
				{
					helper.AddStudy(studyInstanceUid, null, "DICOM_LOCAL");
				}
				helper.OpenStudies();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}