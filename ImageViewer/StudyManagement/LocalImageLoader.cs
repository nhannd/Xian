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
	public class LocalImageLoader
	{
		private int _totalImages;
		private int _failedImages;
		private List<LocalImageSop> _images = new List<LocalImageSop>();

		public LocalImageLoader()
		{
		}

		public List<LocalImageSop> Load(string path, out int totalImages, out int failedImages)
		{
			_totalImages = 0;
			_failedImages = 0;
			_images.Clear();

			FileProcessor.ProcessFile process = new FileProcessor.ProcessFile(LoadImage);
			FileProcessor.Process(path, "*.dcm", process, true);

			failedImages = _failedImages;
			totalImages = _totalImages;

			return _images;
		}

		private void LoadImage(string file)
		{
			LocalImageSop image = null;

			try
			{
				image = new LocalImageSop(file);

				ImageValidator.ValidateSOPInstanceUID(image.SopInstanceUID);
				ImageViewerComponent.StudyManager.StudyTree.AddImage(image);
				_images.Add(image);
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
