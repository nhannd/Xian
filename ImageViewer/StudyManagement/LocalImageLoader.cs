using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
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
				_viewer.EventBroker.OnImageLoaded(
					new SopEventArgs(_viewer.StudyTree.GetSop(image.SopInstanceUID)));
			}
			catch (Exception e)
			{
				// Things that could go wrong in which an exception will be thrown:
				// 1) file is not a valid DICOM image
				// 2) file is a valid DICOM image, but its image parameters are invalid
				// 3) file is a valid DICOM image, but we can't handle this type of DICOM image

				_failedImages++;
				Platform.Log(e, LogLevel.Error);
			}

			_totalImages++;
		}
	}
}
