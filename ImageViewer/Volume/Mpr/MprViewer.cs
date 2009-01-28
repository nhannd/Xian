#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public class MprViewer : IDisposable
	{
		#region Private fields

		private readonly ImageViewerComponent _imageViewer;
		private readonly MprLayoutManager _layoutManager;
		private Volume _volume;

		#endregion

		public MprViewer()
		{
			_layoutManager = new MprLayoutManager();
			_imageViewer = new ImageViewerComponent(_layoutManager);
		}

		private MprLayoutManager LayoutManager
		{
			get { return _layoutManager; }
		}

		private ImageViewerComponent ImageViewer
		{
			get { return _imageViewer; }
		}

		#region Load from DisplaySet

		public static List<List<Frame>> SplitDisplaySet(IDisplaySet displaySet)
		{
			List<Frame> allFrames = new List<Frame>();
			foreach (PresentationImage pi in displaySet.PresentationImages)
			{
				DicomGrayscalePresentationImage dgpi = (DicomGrayscalePresentationImage)pi;
				foreach (Frame frame in dgpi.ImageSop.Frames)
					allFrames.Add(frame);
			}

			List<List<Frame>> frameGroups = VolumeBuilder.SplitFrameGroups(allFrames);
			List<List<Frame>> validatedGroups = new List<List<Frame>>();
			foreach (List<Frame> group in frameGroups)
			{
				string reasonValidateFailed;
				if (VolumeBuilder.ValidateFrames(group, out reasonValidateFailed))
				{
					validatedGroups.Add(group);
				}
			}

			return validatedGroups;
		}

		public void OpenDisplaySet(IDisplaySet displaySet)
		{
			List<Frame> frames = new List<Frame>();
			foreach (PresentationImage pi in displaySet.PresentationImages)
			{
				DicomGrayscalePresentationImage dgpi = (DicomGrayscalePresentationImage)pi;
				foreach (Frame frame in dgpi.ImageSop.Frames)
					frames.Add(frame);
			}

			OpenFrames(frames);
		}

		public void OpenFrames(List<Frame> frames)
		{
			IDesktopWindow desktop = Application.ActiveDesktopWindow;

			string reasonValidateFailed;
			if (VolumeBuilder.ValidateFrames(frames, out reasonValidateFailed) == false)
			{
				desktop.ShowMessageBox("Unable to load Data Set as MPR.\n\nReason:\n" + reasonValidateFailed, MessageBoxActions.Ok);
				return;
			}

			BackgroundTask task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
					{
						BackgroundTaskProgress prog = new BackgroundTaskProgress(10, "Creating Volume...");
						context.ReportProgress(prog);

						_volume = VolumeBuilder.BuildVolume(frames);

						prog = new BackgroundTaskProgress(80, "Creating Sagittal, Coronal, Axial...");
						context.ReportProgress(prog);
						CreateAndLoadSagittalDisplaySet(_volume);
						CreateAndLoadCoronalDisplaySet(_volume);
						CreateAndLoadAxialDisplaySet(_volume);

						context.Complete(null);
					}, true);

			ProgressDialog.Show(task, desktop, true, ProgressBarStyle.Blocks);

			// Inlined below, so that I could control the workspace title
			//ImageViewerComponent.LaunchInActiveWindow(ImageViewer);

			IWorkspace workspace = ApplicationComponent.LaunchAsWorkspace(
				Application.ActiveDesktopWindow,
				ImageViewer,
				"MPR - " + ImageViewer.PatientsLoadedLabel);

			workspace.Closed += delegate { ImageViewer.Dispose(); };
			ImageViewer.Layout();
			ImageViewer.PhysicalWorkspace.SelectDefaultImageBox();
		}

		#endregion

		#region Load from local files

		public void OpenFiles(string[] files)
		{
			bool cancelled = false;
			bool anyFailures = false;
			int successfulImagesInLoadFailure = 0;

			try
			{
				LoadLocalImages(files, Application.ActiveDesktopWindow, out cancelled);
			}
			catch (OpenStudyException e)
			{
				anyFailures = true;
				successfulImagesInLoadFailure = e.SuccessfulImages;
				ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
			}

			if (cancelled || (anyFailures && successfulImagesInLoadFailure == 0))
			{
				Dispose();
				return;
			}

			// Inlined below, so that I could control the workspace title
			//ImageViewerComponent.LaunchInActiveWindow(mprViewer.ImageViewer);

			IWorkspace workspace = ApplicationComponent.LaunchAsWorkspace(
				Application.ActiveDesktopWindow,
				ImageViewer,
				"MPR - " + ImageViewer.PatientsLoadedLabel);

			workspace.Closed += delegate { ImageViewer.Dispose(); };
			ImageViewer.Layout();
			ImageViewer.PhysicalWorkspace.SelectDefaultImageBox();
		}

		private int _totalImages;
		private int _failedImages;

		private void LoadLocalImages(string[] files, IDesktopWindow desktop, out bool cancelled)
		{
			Platform.CheckForNullReference(files, "files");

			_totalImages = 0;
			_failedImages = 0;

			bool userCancelled = false;
			bool volumeFailed = false;
			string reasonVolumeFailed = string.Empty;

			BackgroundTask task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
					{
						int percentComplete = 0;
						for (int i = 0; i < files.Length; i++)
						{
							LoadLocalImage(files[i]);

							percentComplete = (int)(((float)(i + 1) / files.Length) * 40);
							string message1 = String.Format(SR.MessageFormatOpeningImages, i + 1, files.Length);

							BackgroundTaskProgress progress = new BackgroundTaskProgress(percentComplete, message1);
							context.ReportProgress(progress);

							if (context.CancelRequested)
							{
								userCancelled = true;
								break;
							}
						}

						BackgroundTaskProgress prog = new BackgroundTaskProgress(percentComplete, "Creating Volume...");
						context.ReportProgress(prog);

						volumeFailed = ! VolumeBuilder.ValidateFrames(_localFrames, out reasonVolumeFailed);
						if (volumeFailed)
							return;

						_volume = VolumeBuilder.BuildVolume(_localFrames);

						prog = new BackgroundTaskProgress(90, "Creating Sagittal, Coronal, Axial...");
						context.ReportProgress(prog);
						CreateAndLoadSagittalDisplaySet(_volume);
						CreateAndLoadCoronalDisplaySet(_volume);
						CreateAndLoadAxialDisplaySet(_volume);

						context.Complete(null);
					}, true);

			ProgressDialog.Show(task, desktop, true, ProgressBarStyle.Blocks);
			cancelled = userCancelled;

			if (_failedImages > 0)
			{
				string message = String.Format("{0} of {1} images have failed to load.", _failedImages, _totalImages);
				OpenStudyException ex = new OpenStudyException(message);
				ex.TotalImages = _totalImages;
				ex.FailedImages = _failedImages;
				throw ex;
			}

			//ggerade: This all very ugly... probably going away anyways so I'm not worried about it
			if (volumeFailed)
			{
				desktop.ShowMessageBox("Unable to load Data Set as MPR.\n\nReason:\n" + reasonVolumeFailed, MessageBoxActions.Ok);
				cancelled = true;
			}
		}

		private void LoadLocalImage(string file)
		{
			Platform.CheckForNullReference(file, "file");

			try
			{
				ImageSop image = new ImageSop(new LocalSopDataSource(file));

				foreach (Frame frame in image.Frames)
					_localFrames.Add(frame);
			}
			catch (Exception e)
			{
				// Things that could go wrong in which an exception will be thrown:
				// 1) file is not a valid DICOM image
				// 2) file is a valid DICOM image, but its image parameters are invalid
				// 3) file is a valid DICOM image, but we can't handle this type of DICOM image

				_failedImages++;
				Platform.Log(LogLevel.Error, e);
			}

			_totalImages++;
		}

		private readonly List<Frame> _localFrames = new List<Frame>();

		#endregion

		#region DisplaySet creation

		private void CreateAndLoadSagittalDisplaySet(Volume vol)
		{
			DisplaySet displaySet = VolumeSlicer.CreateSagittalDisplaySet(vol);

			AddAllSopsToStudyTree(displaySet);
			LayoutManager.LoadSagittalDisplaySet(displaySet);
		}

		private void CreateAndLoadCoronalDisplaySet(Volume vol)
		{
			DisplaySet displaySet = VolumeSlicer.CreateCoronalDisplaySet(vol);

			AddAllSopsToStudyTree(displaySet);
			LayoutManager.LoadCoronalDisplaySet(displaySet);
		}

		private void CreateAndLoadAxialDisplaySet(Volume vol)
		{
			DisplaySet displaySet = VolumeSlicer.CreateAxialDisplaySet(vol);

			AddAllSopsToStudyTree(displaySet);
			LayoutManager.LoadAxialDisplaySet(displaySet);
		}

		// Note: The overlays expect that a Sop is parented by a Series, so this was the easiest way
		//	to keep the IVC happy.
		private void AddAllSopsToStudyTree(IDisplaySet displaySet)
		{
			// Now load the generated images into the viewer
			foreach (PresentationImage presentationImage in displaySet.PresentationImages)
			{
				DicomGrayscalePresentationImage dicomGrayscalePresentationImage =
					(DicomGrayscalePresentationImage)presentationImage;

				ImageSop sop = dicomGrayscalePresentationImage.ImageSop;
				_imageViewer.StudyTree.AddSop(sop);
			}
		}

		#endregion

		#region Disposal

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				_imageViewer.Dispose();
				_layoutManager.Dispose();
				if (_volume != null)
					_volume.Dispose();
			}
		}

		#endregion
	}
}