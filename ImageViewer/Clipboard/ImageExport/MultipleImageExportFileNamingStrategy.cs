using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	internal class ImageFileNamePair
	{
		public readonly IPresentationImage Image;
		public readonly string FileName;

		public ImageFileNamePair(IPresentationImage image, string fileName)
		{
			this.Image = image;
			this.FileName = fileName;
		}
	}

	internal class MultipleImageExportFileNamingStrategy
	{
		private readonly string _baseDirectory;
		private int _singleImageStartNumber;

		public MultipleImageExportFileNamingStrategy(string baseDirectory)
		{
			_baseDirectory = baseDirectory;
			_singleImageStartNumber = 1;
		}

		public string GetSingleImageFileName(IPresentationImage image, string fileExtension)
		{
			return GetImageFileName(_baseDirectory, "Image", fileExtension, ref _singleImageStartNumber);
		}

		public IEnumerable<ImageFileNamePair> GetImagesAndFileNames(IDisplaySet displaySet, string fileExtension)
		{
			string displaySetDirectory = GetDisplaySetDirectory(displaySet, _baseDirectory);
			Directory.CreateDirectory(displaySetDirectory);

			int startNumber = 1;
			foreach(IPresentationImage image in displaySet.PresentationImages)
			{
				string filePath = GetImageFileName(displaySetDirectory, "Image", fileExtension, ref startNumber);
				yield return new ImageFileNamePair(image, filePath);
			}
		}

		private static string GetDisplaySetDirectoryPrefix(IDisplaySet displaySet)
		{
			string prefix = "DisplaySet";

			if (displaySet.PresentationImages.Count > 0 &&
					displaySet.PresentationImages[0] is IImageSopProvider)
			{
				IImageSopProvider provider = (IImageSopProvider)displaySet.PresentationImages[0];
				string seriesDescription = (provider.ImageSop.SeriesDescription ?? "").Trim();

				if (!String.IsNullOrEmpty(seriesDescription))
				{
					prefix = seriesDescription;
					if (prefix.Length > 16)
						prefix = prefix.Remove(16);
				}
				else
				{
					string modality = (provider.ImageSop.Modality ?? "").Trim();
					if (!String.IsNullOrEmpty(modality))
					{
						prefix = String.Format("{0}{1}", modality, provider.ImageSop.SeriesNumber);
						if (provider.ImageSop.NumberOfFrames > 1)
							prefix = String.Format("{0}.{1}", prefix, provider.ImageSop.InstanceNumber);
					}
				}
			}

			char[] invalidChars = Path.GetInvalidPathChars();
			foreach (char invalidChar in invalidChars)
				prefix = prefix.Replace(invalidChar, ' '); //replace invalid characters with spaces.

			return prefix;
		}

		private static string GetDisplaySetDirectory(IDisplaySet displaySet, string baseDirectory)
		{
			string[] existingDirectories = Directory.GetDirectories(baseDirectory);

			string prefix = GetDisplaySetDirectoryPrefix(displaySet);

			int i = 0;
			string directory = prefix;

			while (true)
			{
				if (!CollectionUtils.Contains(existingDirectories,
				                              delegate(string test)
				                              	{
													//this actually gives the directory name
				                              		string testDirectory = Path.GetFileName(test); 
													return String.Compare(testDirectory, directory, true) == 0;
				                              	}))
				{
					return String.Format("{0}\\{1}", baseDirectory, directory);
				}

				directory = string.Format("{0}{1}", prefix, ++i);
			}
		}

		private static string GetImageFileName(string baseDirectory, string prefix, string fileExtension, ref int startNumber)
		{
			string[] existingFilePaths = Directory.GetFiles(baseDirectory);

			do
			{
				string fileName = String.Format("{0}{1}.{2}", prefix, startNumber++, fileExtension);

				if (!CollectionUtils.Contains(existingFilePaths,
				                              delegate(string testFilePath)
				                              	{
				                              		string testFileName = Path.GetFileName(testFilePath);
				                              		return String.Compare(testFileName, fileName, true) == 0;
				                              	}))
				{
					return String.Format("{0}\\{1}", baseDirectory, fileName);
				}


			} while (true);
		}
	}
}
