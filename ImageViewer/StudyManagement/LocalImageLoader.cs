using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class LocalImageLoader
	{
		private string _studyInstanceUID;
		private bool _firstImage;
		private bool _atLeastOneImageFailedToLoad;
		private bool _oneStudyAlreadyLoaded;

		public LocalImageLoader()
		{

		}

		public string Load(string path)
		{
			//string path;

			//if(Platform.IsUnixPlatform) {
			//    path = "/home/jonathan/ClearCanvas/SampleData/" + studyUID;
			//} else {
			//    path = "C:\\TestImages\\By UID\\" + studyUID;
			//}

			_firstImage = true;
			_atLeastOneImageFailedToLoad = false;
			_studyInstanceUID = "";
			_oneStudyAlreadyLoaded = false;

			FileProcessor.ProcessFile process = new FileProcessor.ProcessFile(AddImage);
			FileProcessor.Process(path, "*.dcm", process, true);

			if (_atLeastOneImageFailedToLoad)
				Platform.ShowMessageBox(SR.ErrorAtLeastOneImageFailedToLoad);

			return _studyInstanceUID;
		}

		private void AddImage(string file)
		{
			if (_oneStudyAlreadyLoaded)
				return;

			LocalImageSop image = null;

			try
			{
				image = new LocalImageSop(file);

				ImageValidator.ValidateStudyInstanceUID(image.StudyInstanceUID);

				if (image.StudyInstanceUID == _studyInstanceUID || _firstImage)
					ImageWorkspace.StudyManager.StudyTree.AddImage(image);
				else
					_oneStudyAlreadyLoaded = true;
			}
			catch (Exception e)
			{
				// Things that could go wrong in which an exception will be thrown:
				// 1) file is not a valid DICOM image
				// 2) file is a valid DICOM image, but its image parameters are invalid
				// 3) file is a valid DICOM image, but we can't handle this type of DICOM image

				_atLeastOneImageFailedToLoad = true;
				Platform.Log(e, LogLevel.Error);
				return;
			}

			// Make a note of the study UID so we know what study we're trying to load
			if (_firstImage)
			{
				if (image != null)
				{
					_studyInstanceUID = image.StudyInstanceUID;
					_firstImage = false;
				}
			}
		}
	}
}
