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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Utilities.Command
{
	/// <summary>
	/// <see cref="CommandBase"/> for deleting a file, not that there is no rollback.  Rollback
	/// should be accomplished by other means.  IE, do a rename of the file, then delete when everything
	/// else is done.
	/// </summary>
	public class FileDeleteCommand : CommandBase, IDisposable
	{
        private string _backupFile;
        private bool _backedUp;
        private readonly string _originalFile;

		public FileDeleteCommand(string path, bool requiresRollback) 
            : base(String.Format("Delete {0}", path), requiresRollback)
		{
            Platform.CheckForNullReference(path, "path");
			_originalFile = path;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
            if (RequiresRollback)
            {
                Backup();
            }

			if (File.Exists(_originalFile))
			{
			    File.Delete(_originalFile);
			}
            else
			{
			    Platform.Log(LogLevel.Warn, "Attempted to delete file which doesn't exist: {0}", _originalFile);
			}
		}

	    private void Backup()
	    {
            if (File.Exists(_originalFile))
            {
                _backupFile = FileUtils.Backup(_originalFile, ProcessorContext.BackupDirectory);
                _backedUp = true;
            }            
	    }

	    protected override void OnUndo()
		{
			if (_backedUp)
			{
			    try
			    {
                    Platform.Log(LogLevel.Debug, "Restoring {0} ...", _originalFile);
                    File.Copy(_backupFile, _originalFile, true);
                    FileUtils.Delete(_backupFile);
			    }
			    catch (Exception e)
			    {
                    Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to backup deleted file: {0}", _backupFile);
			    }
			}
		}

        public override string ToString()
        {
            return String.Format("Delete file {0}", _originalFile);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (File.Exists(_backupFile))
            {
                FileUtils.Delete(_backupFile);
            }
        }

        #endregion
    }
}