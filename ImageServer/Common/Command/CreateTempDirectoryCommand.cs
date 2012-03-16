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
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.Command
{
	/// <summary>
	/// Create a temporary directory.  Remove the directory and its contents when disposed.
	/// </summary>
	public class CreateTempDirectoryCommand : CommandBase, IDisposable
	{
		#region Private Members
		private readonly string _directory;
		private bool _created;
		#endregion

		public string TempDirectory
		{
			get { return _directory; }
		}
		public CreateTempDirectoryCommand()
			: base("Create Temp Directory", true)
		{
		    _directory = Path.Combine(ServerPlatform.TempDirectory, "Archive");
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			if (Directory.Exists(_directory))
			{
				_created = false;
				return;
			}

			try
			{
			    Directory.CreateDirectory(_directory);
			}
            catch(UnauthorizedAccessException)
            {
                //alert the system admin
                ServerPlatform.Alert(AlertCategory.System, AlertLevel.Critical, "Filesystem", AlertTypeCodes.NoPermission, null, TimeSpan.Zero,
                                     "Unauthorized access to {0} from {1}", _directory, ServerPlatform.HostId);
                throw;
            }

			_created = true;
		}

		protected override void OnUndo()
		{
			if (_created)
			{
				DirectoryUtility.DeleteIfExists(_directory);
				_created = false;
			}
		}

		public void Dispose()
		{
			if (_created)
			{
				DirectoryUtility.DeleteIfExists(_directory);
				_created = false;
			}
		}
	}
}
