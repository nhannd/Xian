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
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

// ... (other using namespace statements here)

namespace MyPlugin.DicomImaging
{
	[MenuAction("save", "global-menus/PresentationStates/SavePresentationStateTool", "Save")]
	[ButtonAction("save", "global-toolbars/ToolbarStandard/SavePresentationStateTool", "Save")]
	[IconSet("save", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class SavePresentationStateTool : ImageViewerTool
	{
		private readonly FileExtensionFilter extDcm = new FileExtensionFilter("*.dcm", "DCM Files (*.dcm)");
		private readonly FileExtensionFilter extAll = new FileExtensionFilter("*.*", "All Files (*.*)");
		private string _lastPresentationState = "";

		public void Save()
		{
			FileDialogCreationArgs args = new FileDialogCreationArgs(_lastPresentationState);
			args.Filters.Add(extDcm);
			args.Filters.Add(extAll);
			args.FileExtension = "dcm";
			FileDialogResult result = base.Context.DesktopWindow.ShowSaveFileDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				_lastPresentationState = result.FileName;

				try
				{
					DicomSoftcopyPresentationState presentationState = DicomSoftcopyPresentationState.Create(base.SelectedPresentationImage);
					presentationState.DicomFile.Save(_lastPresentationState);
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "Failed to serialize presentation state.");
					base.Context.DesktopWindow.ShowMessageBox(ex.Message, MessageBoxActions.Ok);
				}
			}
		}
	}
}