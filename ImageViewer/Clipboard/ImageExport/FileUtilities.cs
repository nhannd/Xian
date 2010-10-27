#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	internal static class FileUtilities
	{
		public static bool HasExtension(string fileName, string[] allowedExtensions)
		{
			if (String.IsNullOrEmpty(fileName))
				return false;

			string extension = Path.GetExtension(fileName);
			if (String.IsNullOrEmpty(extension))
				return (allowedExtensions.Length == 0);

			extension = extension.Replace(".", "").Trim();

			return CollectionUtils.Contains(
				allowedExtensions,
				delegate(string test)
					{
						string testExtension = test.Replace(".", "").Trim();
						return String.Compare(extension, testExtension, true) == 0;
					});
		}

		public static string CorrectFileNameExtension(string filePath, string[] allowedExtensions)
		{
			if (String.IsNullOrEmpty(Path.GetFileName(filePath) ?? ""))
				return "";

			if (HasExtension(filePath, allowedExtensions) || allowedExtensions.Length == 0)
				return filePath;

			return String.Format("{0}.{1}", filePath, allowedExtensions[0]);
		}
	}
}
