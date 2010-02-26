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
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.StudyManagement;
using Path=System.IO.Path;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("go", "imageviewer-contextmenu/Change Aspect Ratio", "Go")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ChangePixelAspectRatioTool : ImageViewerTool
	{
		private class NullContext : IBackgroundTaskContext
		{
			#region IBackgroundTaskContext Members

			public object UserState
			{
				get { return null; }
			}

			public bool CancelRequested
			{
				get { return false; }
			}

			public void ReportProgress(BackgroundTaskProgress progress)
			{
			}

			public void Complete(params object[] results)
			{
			}

			public void Cancel()
			{
			}

			public void Error(Exception e)
			{
				throw e;
			}

			#endregion
		}

		private volatile ChangePixelAspectRatioComponent _component;
		private volatile string _outputDirectory;
		private volatile List<string> _dicomFileNames;

		public ChangePixelAspectRatioTool()
		{
		}

		private IEnumerable<IPresentationImage> GetImages(bool singleImage)
		{
			if (singleImage)
			{
				yield return Context.Viewer.SelectedPresentationImage;
			}
			else
			{
				foreach (var displaySet in Context.Viewer.SelectedImageBox.DisplaySet.PresentationImages)
					yield return displaySet;
			}
		}
		
		public void Go()
		{
			_component = new ChangePixelAspectRatioComponent();
			if (ApplicationComponentExitCode.Accepted !=
				ApplicationComponent.LaunchAsDialog(Context.DesktopWindow, _component, "Change Aspect Ratio"))
				return;

			FileDialogResult fileDialog = Context.DesktopWindow.ShowSelectFolderDialogBox(new SelectFolderDialogCreationArgs());
			if (fileDialog.Action != DialogBoxAction.Ok)
				return;

			_outputDirectory = fileDialog.FileName;

			_dicomFileNames = CollectionUtils.Map(GetImages(!_component.ConvertWholeDisplaySet),
						(IPresentationImage image) => ((LocalSopDataSource)((IImageSopProvider)image).ImageSop.DataSource).Filename);

			try
			{
				if (_dicomFileNames.Count > 5)
				{
					var task = new BackgroundTask(Go, true);
					ProgressDialog.Show(task, Context.DesktopWindow, true, ProgressBarStyle.Continuous);
				}
				else
				{
					BlockingOperation.Run(() => Go(new NullContext()));
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, Context.DesktopWindow);
			}
		}

		private void Go(IBackgroundTaskContext context)
		{
			string studyUid = DicomUid.GenerateUid().UID;
			string seriesUid = DicomUid.GenerateUid().UID;

			PixelAspectRatioChanger changer = 
				new PixelAspectRatioChanger
              	{
              		IncreasePixelDimensions = _component.IncreasePixelDimensions,
              		NewAspectRatio = new PixelAspectRatio(_component.AspectRatioRow, _component.AspectRatioColumn),
              		RemoveCalibration = _component.RemoveCalibration
              	};

			int i = 0;
			context.ReportProgress(new BackgroundTaskProgress(i, _dicomFileNames.Count, "Exporting ..."));

			try
			{
				foreach (string originalFile in _dicomFileNames)
				{
					var file = new DicomFile(originalFile);
					file.Load(DicomReadOptions.None);

					string sopInstanceUid = DicomUid.GenerateUid().UID;

					file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(studyUid);
					file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(seriesUid);
					file.DataSet[DicomTags.SopInstanceUid].SetStringValue(sopInstanceUid);

					changer.ChangeAspectRatio(file);

					string outputFileName = Path.Combine(_outputDirectory, String.Format("{0}.dcm", sopInstanceUid));
					file.Save(outputFileName);

					if (context.CancelRequested)
					{
						context.Cancel();
						return;
					}

					context.ReportProgress(new BackgroundTaskProgress(++i, _dicomFileNames.Count + 1, "Exporting ..."));
				}
			}
			catch (Exception e)
			{
				context.Error(e);
				return;
			}

			context.Complete();
		}


	}
}