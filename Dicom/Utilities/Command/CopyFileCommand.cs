﻿#region License

// Copyright (c) 2012, ClearCanvas Inc.
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
    public class CopyFileCommand : CommandBase, IDisposable
    {
        #region Private Members
        private readonly string _sourceFile;
        private readonly string _destinationFile;
        private string _destBackupFile;
        private readonly bool _failIfExists;
        #endregion

        public CopyFileCommand(string sourceFile, string destinationFile, bool failIfExists)
            : base(String.Format("Copy {0} to {1}", sourceFile, destinationFile), true)
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

            if (File.Exists(_destinationFile))
                FileUtils.Copy(_sourceFile, _destinationFile, !_failIfExists);
            else
                FileUtils.Copy(_sourceFile, _destinationFile, false);

            try
            {
                if ((File.GetAttributes(_destinationFile) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(_destinationFile, FileAttributes.Normal);
            }
            catch (Exception)
            { }
        }

        private void Backup()
        {
            if (File.Exists(_destinationFile))
            {
                _destBackupFile = FileUtils.Backup(_sourceFile, ProcessorContext.BackupDirectory);
            }
        }

        protected override void OnUndo()
        {
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
                    Platform.Log(LogLevel.Warn, "Error occured when rolling back destination file in CopyFileCommand: {0}", e.Message);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (File.Exists(_destBackupFile))
                File.Delete(_destBackupFile);
        }

        #endregion
    }
}
