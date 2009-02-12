using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;

#if DEBUG

namespace ClearCanvas.ImageViewer.PresentationStates
{
	[MenuAction("save", "global-menus/Test Presentation State/Save PR Object", "SavePR")]
	[MenuAction("load", "global-menus/Test Presentation State/Load PR Object", "LoadPR")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal class PresentationStateTestTool : ImageViewerTool
	{
		private readonly FileExtensionFilter extDcm = new FileExtensionFilter("*.dcm", "Dicom Files (*.dcm)");
		private readonly FileExtensionFilter extPre = new FileExtensionFilter("*.pre", "PRE Files (*.pre)");
		private readonly FileExtensionFilter extAll = new FileExtensionFilter("*.*", "All Files (*.*)");
		private string _lastPRFile = "";

		public void SavePR()
		{
			FileDialogCreationArgs args = new FileDialogCreationArgs(_lastPRFile);
			args.Filters.Add(extDcm);
			args.Filters.Add(extPre);
			args.Filters.Add(extAll);
			args.FileExtension = "dcm";
			FileDialogResult result = base.Context.DesktopWindow.ShowSaveFileDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				_lastPRFile = result.FileName;

				try
				{
					DicomSoftcopyPresentationState presentationState = DicomSoftcopyPresentationState.Create(base.SelectedPresentationImage);
					presentationState.DicomFile.Save(_lastPRFile);
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
			args.Filters.Add(extPre);
			args.Filters.Add(extAll);
			FileDialogResult result = base.Context.DesktopWindow.ShowOpenFileDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				DicomFile dcf = new DicomFile(_lastPRFile = result.FileName);
				dcf.Load();

				try
				{
					DicomSoftcopyPresentationState presentationState = DicomSoftcopyPresentationState.Load(dcf);
					presentationState.Deserialize(base.SelectedPresentationImage);

					// automatically install a lut from the presentation state if applicable
					if (base.SelectedPresentationImage is IDicomVoiLutsProvider && base.SelectedPresentationImage is IVoiLutProvider)
					{
						IDicomVoiLutsProvider voiLutsProvider = (IDicomVoiLutsProvider) base.SelectedPresentationImage;

						if (voiLutsProvider.DicomVoiLuts.PresentationVoiDataLuts.Count > 0)
						{
							VoiDataLut dataLutIod = voiLutsProvider.DicomVoiLuts.PresentationVoiDataLuts[0];
							AdjustableDataLut dataLut = new AdjustableDataLut(
								new SimpleDataLut(dataLutIod.FirstMappedPixelValue,
								                  dataLutIod.Data,
								                  dataLutIod.MinOutputValue,
								                  dataLutIod.MaxOutputValue,
								                  dataLutIod.ToString(),
								                  dataLutIod.Explanation));
							base.SelectedVoiLutProvider.VoiLutManager.InstallLut(dataLut);
							base.SelectedPresentationImage.Draw();
						}
						else if (voiLutsProvider.DicomVoiLuts.PresentationVoiLinearLuts.Count > 0)
						{
							Window window = voiLutsProvider.DicomVoiLuts.PresentationVoiLinearLuts[0];
							BasicVoiLutLinear linearLut = new BasicVoiLutLinear(window.Width, window.Center);
							base.SelectedVoiLutProvider.VoiLutManager.InstallLut(linearLut);
							base.SelectedPresentationImage.Draw();
						}
					}
				}
				catch (Exception ex)
				{
					base.Context.DesktopWindow.ShowMessageBox(ex.Message, MessageBoxActions.Ok);
				}
			}
		}
	}
}

#endif