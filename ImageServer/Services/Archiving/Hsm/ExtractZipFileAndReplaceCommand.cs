#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common.Command;
using Ionic.Zip;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// Class for extracting a file from a zip over the top of another file, and preserving
	/// the old file for restoring on failure.
	/// </summary>
	public class ExtractZipFileAndReplaceCommand : CommandBase, IDisposable
	{
		private readonly string _zipFile;
		private readonly string _destinationFolder;
		private readonly string _sourceFile;
		private bool _fileBackedup;
		private string _storageFile = String.Empty;
		private string _backupFile = String.Empty;

		public ExtractZipFileAndReplaceCommand(string zipFile, string sourceFile, string destinationFolder)
			: base("Extract file from Zip and replace existing file", true)
		{
			_zipFile = zipFile;
			_destinationFolder = destinationFolder;
			_sourceFile = sourceFile;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			_storageFile = Path.Combine(_destinationFolder, _sourceFile);
			_backupFile = Path.Combine(ProcessorContext.TempDirectory, _sourceFile);


			string baseDirectory = _backupFile.Substring(0, _backupFile.LastIndexOfAny(new [] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar }));
			if (!Directory.Exists(baseDirectory))
				Directory.CreateDirectory(baseDirectory);

			if (File.Exists(_storageFile))
			{
				File.Move(_storageFile, _backupFile);
				_fileBackedup = true;
			}
			using (var zip = new ZipFile(_zipFile))
			{
				zip.Extract(_sourceFile, _destinationFolder, true);
			}
		}

		protected override void OnUndo()
		{
			if (_fileBackedup)
			{
				if (File.Exists(_storageFile))
					File.Delete(_storageFile);
				File.Move(_backupFile, _storageFile);
				_fileBackedup = false;
			}
		}

		public void Dispose()
		{
			if (File.Exists(_backupFile))
				File.Delete(_backupFile);
		}
	}
}
