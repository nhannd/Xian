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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>C:\Projects\Jin\Xian\ImageServer\Common\Command\DeleteFileCommand.cs
    /// Command to delete a file.
    /// </summary>
    public class DeleteFileCommand : CommandBase, IDisposable
    {
        #region Private Memebers
        private readonly string _originalFile;
        private string _backupFile;
        #endregion

        #region Constructors
        public DeleteFileCommand(String file)
            :base(String.Format("Delete {0}", file), true)
        {
            _originalFile = file;
        }
        #endregion

        #region Overriden Protected Methods
		protected override void OnExecute(CommandProcessor theProcessor)
        {
            if (RequiresRollback)
            {
                Backup();
            }

            var fileInfo = new FileInfo(_originalFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        protected override void OnUndo()
        {
            if (!File.Exists(_originalFile))
            {
                File.Copy(_backupFile, _originalFile, true);
            }
        }
        #endregion

        #region Private Methods

        private void Backup()
        {
            if (File.Exists(_originalFile))
            {
                _backupFile = FileUtils.Backup(_originalFile,ProcessorContext.BackupDirectory);
            }
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            if (RollBackRequested)
            {
                if (File.Exists(_originalFile))
                {
                    // we can now safely delete the backup file
                    if (File.Exists(_backupFile))
                        FileUtils.Delete(_backupFile);
                }
            }
            else
            {
                if (File.Exists(_backupFile))
					FileUtils.Delete(_backupFile);
            }
        }

        #endregion
    }
}
