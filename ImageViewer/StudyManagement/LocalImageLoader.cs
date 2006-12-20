using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common.Utilities;

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

		public void Load(string path)
		{
			_totalImages = 0;
			_failedImages = 0;

			FileProcessor.ProcessFile process = new FileProcessor.ProcessFile(LoadImage);
			FileProcessor.Process(path, "*.dcm", process, true);
		}

		private void LoadImage(string file)
		{
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
