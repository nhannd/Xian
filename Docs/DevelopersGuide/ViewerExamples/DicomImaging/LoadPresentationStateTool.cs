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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

// ... (other using namespace statements here)

namespace MyPlugin.DicomImaging
{
	[MenuAction("load", "global-menus/PresentationStates/LoadPresentationStateTool", "Load")]
	[ButtonAction("load", "global-toolbars/ToolbarStandard/LoadPresentationStateTool", "Load")]
	[IconSet("load", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class LoadPresentationStateTool : ImageViewerTool
	{
		private readonly FileExtensionFilter extDcm = new FileExtensionFilter("*.dcm", "DCM Files (*.dcm)");
		private readonly FileExtensionFilter extAll = new FileExtensionFilter("*.*", "All Files (*.*)");
		private string _lastPresentationState = "";

		public void Load()
		{
			FileDialogCreationArgs args = new FileDialogCreationArgs(_lastPresentationState);
			args.Filters.Add(extDcm);
			args.Filters.Add(extAll);
			FileDialogResult result = base.Context.DesktopWindow.ShowOpenFileDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				DicomFile dcf = new DicomFile(_lastPresentationState = result.FileName);
				dcf.Load();

				try
				{
					DicomSoftcopyPresentationState presentationState = DicomSoftcopyPresentationState.Load(dcf);
					((IDicomPresentationImage)base.SelectedPresentationImage).PresentationState = presentationState;
					base.SelectedPresentationImage.Draw();

					if (SelectedVoiLutProvider != null)
					{
						// it is up to the client code to choose a LUT that should be applied, if desired.
						// this method gets the first available LUT, giving priority to the presentation state over the image
						IComposableLut lut = InitialVoiLutProvider.Instance.GetLut(base.SelectedPresentationImage);
						base.SelectedVoiLutProvider.VoiLutManager.InstallLut(lut);
						base.SelectedPresentationImage.Draw();
					}
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "Failed to deserialize presentation state.");
					base.Context.DesktopWindow.ShowMessageBox(ex.Message, MessageBoxActions.Ok);
				}
			}
		}
	}
}