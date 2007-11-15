#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal class LocalImageLoader
	{
		private IImageViewer _viewer;

		private int _totalImages;
		private int _failedImages;

		public LocalImageLoader(IImageViewer viewer)
		{
			_viewer = viewer;
		}

		public int TotalImages
		{
			get { return _totalImages; }
		}
		
		public int FailedImages
		{
			get { return _failedImages; }
		}

		public void Load(string[] files, IDesktopWindow desktop, out bool cancelled)
		{
			Platform.CheckForNullReference(files, "files");

			_totalImages = 0;
			_failedImages = 0;

			bool userCancelled = false;

			if (desktop != null)
			{
				BackgroundTask task = new BackgroundTask(
					delegate(IBackgroundTaskContext context)
					{
						for (int i = 0; i < files.Length; i++)
						{
							LoadImage(files[i]);

							int percentComplete = (int)(((float)(i + 1) / files.Length) * 100);
							string message = String.Format("Opening {0} of {1} images", i, files.Length);

							BackgroundTaskProgress progress = new BackgroundTaskProgress(percentComplete, message);
							context.ReportProgress(progress);

							if (context.CancelRequested)
							{
								userCancelled = true;
								break;
							}
						}

						context.Complete(null);

					}, true);

                ProgressDialog.Show(task, desktop, true, ProgressBarStyle.Blocks);
				cancelled = userCancelled;
			}
			else
			{
				foreach (string file in files)
					LoadImage(file);

				cancelled = false;
			}
		}

		private void LoadImage(string file)
		{
			Platform.CheckForNullReference(file, "file");

			LocalImageSop image = null;

			try
			{
				image = new LocalImageSop(file);

				_viewer.StudyTree.AddImage(image);
				_viewer.EventBroker.OnImageLoaded(new ItemEventArgs<Sop>(_viewer.StudyTree.GetSop(image.SopInstanceUID)));
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
	}
}
