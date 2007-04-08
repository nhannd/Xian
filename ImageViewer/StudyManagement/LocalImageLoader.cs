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
		private IDesktopWindow _desktop;

		private int _totalImages;
		private int _failedImages;


		public LocalImageLoader(IImageViewer viewer, IDesktopWindow desktop)
		{
			_viewer = viewer;
			_desktop = desktop;
		}

		public int TotalImages
		{
			get { return _totalImages; }
		}
		
		public int FailedImages
		{
			get { return _failedImages; }
		}

		public void Load(string[] files)
		{
			Platform.CheckForNullReference(files, "files");

			_totalImages = 0;
			_failedImages = 0;

			if (_desktop != null)
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
								break;
						}

						context.Complete(null);

					}, true);

				ProgressDialog.Show(task, true, ProgressBarStyle.Blocks, _desktop);
			}
			else
			{
				foreach (string file in files)
					LoadImage(file);
			}
		}

		private void LoadImage(string file)
		{
			Platform.CheckForNullReference(file, "file");

			LocalImageSop image = null;

			try
			{
				image = new LocalImageSop(file);

				ImageValidator.ValidateSOPInstanceUID(image.SopInstanceUID);
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
