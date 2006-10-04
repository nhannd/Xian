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
		private List<string> _studyInstanceUIDs;
		private bool _atLeastOneImageFailedToLoad;
		private bool _studyCouldNotBeLoaded;

		private Exception _innerException;

		public LocalImageLoader()
		{
			_studyInstanceUIDs = new List<string>();
		}

		public IEnumerable<string> Load(IEnumerable<string> paths)
		{
			_atLeastOneImageFailedToLoad = false;
			_studyCouldNotBeLoaded = false;

			_studyInstanceUIDs.Clear();

			foreach (string path in paths)
			{
				FileProcessor.ProcessFile process = new FileProcessor.ProcessFile(AddImage);
				FileProcessor.Process(path, "*.dcm", process, true);

				if (_atLeastOneImageFailedToLoad)
				{
					OpenStudyException e = new OpenStudyException("An error occurred while opening the study(s)", _innerException);

					e.AtLeastOneImageFailedToLoad = _atLeastOneImageFailedToLoad;
					e.StudyCouldNotBeLoaded = _studyCouldNotBeLoaded;

					throw e;
				}
			}

			return _studyInstanceUIDs.AsReadOnly();
		}

		private void AddImage(string file)
		{
			LocalImageSop image = null;

			try
			{
				image = new LocalImageSop(file);

				ImageValidator.ValidateStudyInstanceUID(image.StudyInstanceUID);
				ImageViewerComponent.StudyManager.StudyTree.AddImage(image);
			}
			catch (Exception e)
			{
				// Things that could go wrong in which an exception will be thrown:
				// 1) file is not a valid DICOM image
				// 2) file is a valid DICOM image, but its image parameters are invalid
				// 3) file is a valid DICOM image, but we can't handle this type of DICOM image

				_innerException = e;

				_atLeastOneImageFailedToLoad = true;
				_studyCouldNotBeLoaded = (image.StudyInstanceUID == "");

				Platform.Log(e, LogLevel.Error);
				return;
			}

			if (!_studyInstanceUIDs.Contains(image.StudyInstanceUID))
				_studyInstanceUIDs.Add(image.StudyInstanceUID);
		}
	}
}
