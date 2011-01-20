#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using Ionic.Zip;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// <see cref="ServerCommand"/> for extracting a zip file containing study files to a specific directory.
	/// </summary>
	public class ExtractZipCommand : ServerCommand
	{
		private readonly string _zipFile;
		private readonly string _destinationFolder;
		private readonly bool _overwrite;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="zip">The zip file to extract.</param>
		/// <param name="destinationFolder">The destination folder.</param>
		public ExtractZipCommand(string zip, string destinationFolder): base("Extract Zip File",true)
		{
			_zipFile = zip;
			_destinationFolder = destinationFolder;
			_overwrite = false;
		}

		/// <summary>
		/// Do the unzip.
		/// </summary>
		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			using (ZipFile zip = new ZipFile(_zipFile))
			{
				zip.ExtractAll(_destinationFolder,_overwrite);
			}
		}

		/// <summary>
		/// Undo.  Remove the destination folder.
		/// </summary>
		protected override void OnUndo()
		{
			Directory.Delete(_destinationFolder, true);
		}
	}
}
