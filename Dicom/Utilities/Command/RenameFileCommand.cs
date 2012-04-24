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
	/// A ServerCommand derived class for renaming a file.
	/// </summary>
	public class RenameFileCommand : CommandBase, IDisposable
	{
		#region Private Members
		private readonly string _sourceFile;
		private readonly string _destinationFile;
        private string _srcBackupFile; 
        private string _destBackupFile;
	    private readonly bool _failIfExists;
	    private bool _sourceRenamed;

	    #endregion

		public RenameFileCommand(string sourceFile, string destinationFile, bool failIfExists)
			: base(String.Format("Rename {0} to {1}", sourceFile, destinationFile), true)
		{
			Platform.CheckForNullReference(sourceFile, "Source filename");
			Platform.CheckForNullReference(destinationFile, "Destination filename");
		    
			_sourceFile = sourceFile;
			_destinationFile = destinationFile;
		    _failIfExists = failIfExists;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
            Platform.CheckTrue(File.Exists(_sourceFile), String.Format("Source file '{0}' doesn't exist", _sourceFile));
            
            if (File.Exists(_destinationFile))
            {
                if (_failIfExists)
                    throw new ApplicationException(String.Format("Destination file already exists: {0}", _destinationFile));
            }

            if (RequiresRollback)
                Backup();

		    FileUtils.Copy(_sourceFile, _destinationFile, !_failIfExists);

		    try
		    {
                if ((File.GetAttributes(_destinationFile) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(_destinationFile, FileAttributes.Normal);
		    }
		    catch (Exception)
		    { }

            if (File.Exists(_sourceFile))
                FileUtils.Delete(_sourceFile);
            
		    _sourceRenamed = true;
		}

	    private void Backup()
        {
			//backup source
            _srcBackupFile = FileUtils.Backup(_sourceFile, ProcessorContext.BackupDirectory);

            if (File.Exists(_destinationFile))
            {
                _destBackupFile = FileUtils.Backup(_destinationFile, ProcessorContext.BackupDirectory);
            }
        }

		protected override void OnUndo()
		{
            // restore the source
            if (File.Exists(_srcBackupFile))
            {
                if (_sourceRenamed)
                {
                    try
                    {
                        Platform.Log(LogLevel.Info, "Restoring {0}", _sourceFile);
                        FileUtils.Copy(_srcBackupFile, _sourceFile, true);
                    }
                    catch(Exception e)
                    {
                        Platform.Log(LogLevel.Error, "Error occured when rolling back source file in RenameFileCommand: {0}", e.Message);
                    }
                }
            }

		    // restore destination
            if (File.Exists(_destBackupFile))
            {
                try
                {
                    Platform.Log(LogLevel.Error, "Restoring {0}", _destinationFile);
                    FileUtils.Copy(_destBackupFile, _destinationFile, true);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, "Error occured when rolling back destination file in RenameFileCommand: {0}", e.Message);
                } 
            }
			
		}

        #region IDisposable Members

        public void Dispose()
        {
            if (File.Exists(_srcBackupFile))
                File.Delete(_srcBackupFile); 
            
            if (File.Exists(_destBackupFile))
                File.Delete(_destBackupFile);

        }

        #endregion
    }
}