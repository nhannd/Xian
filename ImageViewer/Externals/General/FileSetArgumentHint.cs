#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;

namespace ClearCanvas.ImageViewer.Externals.General
{
	public class FileSetArgumentHint : IArgumentHint
	{
		private IList<FileInfo> _fileInfos;

		public FileSetArgumentHint(IEnumerable<string> filenames) : this(EnumerateFiles(filenames)) {}

		public FileSetArgumentHint(IEnumerable<FileInfo> fileInfos)
		{
			_fileInfos = new List<FileInfo>(fileInfos).AsReadOnly();
		}

		public ArgumentHintValue this[string key]
		{
			get
			{
				Converter<FileInfo, string> converter;
				switch (key)
				{
					case "FILENAME":
						converter = delegate(FileInfo fileinfo) { return fileinfo.FullName; };
						break;
					case "DIRECTORY":
						converter = delegate(FileInfo fileinfo) { return fileinfo.DirectoryName; };
						break;
					case "FILENAMEONLY":
						converter = delegate(FileInfo fileinfo) { return fileinfo.Name; };
						break;
					case "EXTENSIONONLY":
						converter = delegate(FileInfo fileinfo) { return fileinfo.Extension; };
						break;
					default:
						return ArgumentHintValue.Empty;
				}

				List<string> list = new List<string>();
				foreach (FileInfo fileInfo in _fileInfos)
				{
					string result = converter(fileInfo);
					if (!list.Contains(result))
						list.Add(result);
				}
				return new ArgumentHintValue(list.ToArray());
			}
		}

		public void Dispose() {}

		private static IEnumerable<FileInfo> EnumerateFiles(IEnumerable<string> filenames)
		{
			foreach (string filename in filenames)
				yield return new FileInfo(filename);
		}
	}
}