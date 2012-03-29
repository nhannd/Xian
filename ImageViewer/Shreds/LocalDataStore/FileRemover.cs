#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	public class FileRemover
	{
		private readonly string _topLevelDirectory;
		private readonly List<string> _directoriesToDelete;

		public FileRemover(string topLevelDirectory)
		{
			Platform.CheckForEmptyString(topLevelDirectory, "topLevelDirectory");
			_topLevelDirectory = Path.GetDirectoryName(topLevelDirectory);
			_directoriesToDelete = new List<string>();
		}

		public void DeleteFilesInStudy(IStudy studyToRemove)
		{
			DeleteSopInstanceFiles(studyToRemove.GetSopInstances());
		}

		public void CleanupEmptyDirectories()
		{
			DeleteEmptyDirectories(_directoriesToDelete);
		}

		private void DeleteSopInstanceFiles(IEnumerable<ISopInstance> sopInstancesToDelete)
		{
			foreach (ISopInstance sop in sopInstancesToDelete)
			{
				DeleteFileForSopInstance(sop);

				string directoryName = Path.GetDirectoryName(sop.GetLocationUri());
				if (_directoriesToDelete.Contains(directoryName) == false)
					_directoriesToDelete.Add(directoryName);
			}
		}

		private static void DeleteFileForSopInstance(ISopInstance sopIntanceToDelete)
		{
            if (File.Exists(sopIntanceToDelete.GetLocationUri()) == false)
				return;

			string fileName = sopIntanceToDelete.GetLocationUri();
			if (File.Exists(fileName))
				File.Delete(fileName);
		}

		private void DeleteEmptyDirectories(List<string> directoriesToDelete)
		{
			if (directoriesToDelete.Count == 0)
				return;

			// Subdirectories will always be longer than parent directories
			// sort in descending order based on directory length
			directoriesToDelete.Sort();
			directoriesToDelete.Reverse();

			List<string> parentDirectoriesToDelete = new List<string>();
			foreach (string directoryName in directoriesToDelete)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
				if (directoryInfo.FullName != _topLevelDirectory && IsEmptyDirectory(directoryInfo))
				{
					if (directoryInfo.Parent != null && !parentDirectoriesToDelete.Contains(directoryInfo.Parent.FullName))
						parentDirectoriesToDelete.Add(directoryInfo.Parent.FullName);

					directoryInfo.Delete(true);
				}
			}

			DeleteEmptyDirectories(parentDirectoriesToDelete);
		}

		private static bool IsEmptyDirectory(DirectoryInfo directoryInfo)
		{
			// if the directory doesn't exist, it's not empty
			// if the directory contains at least one file, it's not empty
			if (!directoryInfo.Exists || directoryInfo.GetFiles().Length > 0)
				return false;

			// otherwise, loop through each subdirectory
			foreach (var directory in directoryInfo.GetDirectories())
			{
				// if the subdirectory is not empty, then this directory isn't empty either
				if (!IsEmptyDirectory(directory))
					return false;
			}

			// otherwise, if all subdirectories are empty, then this directory is empty
			return true;
		}
	}
}