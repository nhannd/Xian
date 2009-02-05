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
	/// <summary>
	/// This class allows for loading of a Volume from a DisplaySet, list of Frames, or
	/// list of local files. It currently relies on the VolumeBuilder class both to validate
	/// that a list of frames are able to be represented as a volume and to generate
	/// the volume from a list of frames.
	/// </summary>
	internal class VolumeLoader
	{
		#region Load from DisplaySet

		public Volume LoadFromDisplaySet(IDisplaySet displaySet)
		{
			List<Frame> frames = new List<Frame>();
			foreach (PresentationImage pi in displaySet.PresentationImages)
			{
				IImageSopProvider dgpi = (IImageSopProvider)pi;
				foreach (Frame frame in dgpi.ImageSop.Frames)
					frames.Add(frame);
			}

			return LoadFromFrames(frames);
		}

		private static Volume LoadFromFrames(List<Frame> frames)
		{
			IDesktopWindow desktop = Application.ActiveDesktopWindow;

			string reasonValidateFailed;
			if (VolumeBuilder.ValidateFrames(frames, out reasonValidateFailed) == false)
			{
				desktop.ShowMessageBox("Unable to load Data Set as MPR.\n\nReason:\n" + reasonValidateFailed, MessageBoxActions.Ok);
				//ggerade ToDo: Throw ex
				return null;
			}

			Volume volume = null;

			BackgroundTask task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
					{
						BackgroundTaskProgress prog = new BackgroundTaskProgress(10, "Creating Volume...");
						context.ReportProgress(prog);

						volume = VolumeBuilder.BuildVolume(frames);

						context.Complete(null);
					}, true);

			ProgressDialog.Show(task, desktop, true, ProgressBarStyle.Blocks);

			return volume;
		}

		// experimental utility method to split a displayset into MPR-able subsets
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

		#endregion

		#region Load from local files

		public Volume LoadFromFiles(string[] files)
		{
			Volume volume = null;
			bool cancelled = false;
			bool anyFailures = false;
			int successfulImagesInLoadFailure = 0;

			try
			{
				volume = LoadLocalImages(files, Application.ActiveDesktopWindow, out cancelled);
			}
			catch (OpenStudyException e)
			{
				anyFailures = true;
				successfulImagesInLoadFailure = e.SuccessfulImages;
				ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
			}

			if (cancelled || (anyFailures && successfulImagesInLoadFailure == 0))
				return null;

			return volume;
		}

		private readonly List<Frame> _localFrames = new List<Frame>();
		private int _totalImages;
		private int _failedImages;

		private Volume LoadLocalImages(string[] files, IDesktopWindow desktop, out bool cancelled)
		{
			Platform.CheckForNullReference(files, "files");

			_totalImages = 0;
			_failedImages = 0;

			Volume volume = null;
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
							string message1 = String.Format(ImageViewer.SR.MessageFormatOpeningImages, i + 1, files.Length);

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
						//ggerade ToDo: Throw exception
						if (volumeFailed)
							return;

						volume = VolumeBuilder.BuildVolume(_localFrames);

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

			if (volumeFailed)
			{
				desktop.ShowMessageBox("Unable to load Data Set as MPR.\n\nReason:\n" + reasonVolumeFailed, MessageBoxActions.Ok);
				cancelled = true;
			}

			return volume;
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

		#endregion
	}
}