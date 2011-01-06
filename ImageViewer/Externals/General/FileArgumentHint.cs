#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;

namespace ClearCanvas.ImageViewer.Externals.General
{
	public class FileArgumentHint : IArgumentHint
	{
		private FileInfo _fileInfo;

		public FileArgumentHint(FileInfo fileInfo)
		{
			_fileInfo = fileInfo;
		}

		public FileArgumentHint(string filename) : this(new FileInfo(filename)) {}

		public ArgumentHintValue this[string key]
		{
			get
			{
				switch (key)
				{
					case "FILENAME":
						return new ArgumentHintValue(_fileInfo.FullName);
					case "DIRECTORY":
						return new ArgumentHintValue(_fileInfo.DirectoryName);
					case "FILENAMEONLY":
						return new ArgumentHintValue(_fileInfo.Name);
					case "EXTENSIONONLY":
						return new ArgumentHintValue(_fileInfo.Extension);
				}
				return ArgumentHintValue.Empty;
			}
		}

		public void Dispose() {}
	}
}