#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.IO;
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
			return GetImageFileName(_baseDirectory, "Image", fileExtension, 0, ref _singleImageStartNumber);
		}

		public IEnumerable<ImageFileNamePair> GetImagesAndFileNames(IDisplaySet displaySet, string fileExtension)
		{
			string displaySetDirectory = GetDisplaySetDirectory(displaySet, _baseDirectory);
			Directory.CreateDirectory(displaySetDirectory);

			int numberOfZeroes = displaySet.PresentationImages.Count.ToString().Length;

			int startNumber = 1;
			foreach(IPresentationImage image in displaySet.PresentationImages)
			{
				string filePath = GetImageFileName(displaySetDirectory, "Image", fileExtension, numberOfZeroes, ref startNumber);
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

			//replace directory separators with spaces.
			prefix = prefix.Replace('\\', ' ');
			prefix = prefix.Replace('/', ' ');

			//Replace the wildcard characters as well.
			prefix = prefix.Replace('*', ' ');
			prefix = prefix.Replace('?', ' ');

			char[] invalidChars = Path.GetInvalidPathChars();
			foreach (char invalidChar in invalidChars)
				prefix = prefix.Replace(invalidChar, ' '); //replace invalid characters with spaces.

			return prefix.Trim();
		}

		private static string GetDisplaySetDirectory(IDisplaySet displaySet, string baseDirectory)
		{
			string prefix = GetDisplaySetDirectoryPrefix(displaySet);

			int i = 0;
			string directory = prefix;

			while (true)
			{
				string path = String.Format("{0}\\{1}", baseDirectory, directory);
				if (!Directory.Exists(path))
					return path;

				directory = string.Format("{0}{1}", prefix, ++i);
			}
		}

		private static string GetImageFileName(string baseDirectory, string prefix, string fileExtension, int numberOfZeros, ref int startNumber)
		{
			do
			{
				string numericPortion;
				if (numberOfZeros == 0)
				{
					numericPortion = startNumber.ToString();
				}
				else
				{
					string format = new string('0', numberOfZeros);
					numericPortion = startNumber.ToString(format);
				}

				string path = String.Format("{0}\\{1}{2}.{3}", baseDirectory, prefix, numericPortion, fileExtension);
				if (!File.Exists(path))
					return path;

				++startNumber;

			} while (true);
		}
	}
}
