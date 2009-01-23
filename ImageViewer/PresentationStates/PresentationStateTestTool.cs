using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.PresentationStates
{
#if DEBUG

	[MenuAction("save", "global-menus/Test Presentation State/Save PR Object", "SavePR")]
	[MenuAction("load", "global-menus/Test Presentation State/Load PR Object", "LoadPR")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal class PresentationStateTestTool : ImageViewerTool
	{
		private readonly FileExtensionFilter extDcm = new FileExtensionFilter("*.dcm", "Dicom Files (*.dcm)");
		private readonly FileExtensionFilter extAll = new FileExtensionFilter("*.*", "All Files (*.*)");
		private string _lastPRFile = "";

		public void SavePR()
		{
			FileDialogCreationArgs args = new FileDialogCreationArgs(_lastPRFile);
			args.Filters.Add(extDcm);
			args.Filters.Add(extAll);
			args.FileExtension = "dcm";
			FileDialogResult result = base.Context.DesktopWindow.ShowSaveFileDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				_lastPRFile = result.FileName;

				try
				{
					DicomSoftcopyPresentationState presentationState = DicomSoftcopyPresentationState.Create(base.SelectedPresentationImage);
					presentationState.Save().Save(_lastPRFile);
				}
				catch (Exception ex)
				{
					base.Context.DesktopWindow.ShowMessageBox(ex.Message, MessageBoxActions.Ok);
				}
			}
		}

		public void LoadPR()
		{
			FileDialogCreationArgs args = new FileDialogCreationArgs(_lastPRFile);
			args.Filters.Add(extDcm);
			args.Filters.Add(extAll);
			FileDialogResult result = base.Context.DesktopWindow.ShowOpenFileDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				DicomFile dcf = new DicomFile(_lastPRFile = result.FileName);
				dcf.Load();

				try
				{
					DicomSoftcopyPresentationState presentationState = DicomSoftcopyPresentationState.Load(dcf);
					presentationState.Apply(base.SelectedPresentationImage);
				}
				catch (Exception ex)
				{
					base.Context.DesktopWindow.ShowMessageBox(ex.Message, MessageBoxActions.Ok);
				}
			}
		}
	}

#endif
}
