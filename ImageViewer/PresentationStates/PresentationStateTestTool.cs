#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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