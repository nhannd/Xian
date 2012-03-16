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
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.Command
{
    public class CopyDirectoryCommand : CommandBase, IDisposable
    {
        #region Private Members
        private readonly RateStatistics _copySpeed = new RateStatistics("CopySpeed", RateType.BYTES);
        private readonly string _src;
        private readonly string _dest;
        private readonly DirectoryUtility.CopyProcessCallback _callback;
        private readonly TimeSpanStatistics _backupTime = new TimeSpanStatistics();
        private bool _copied;
        private string _backupDestDir; 
        #endregion

        #region Constructors
        public CopyDirectoryCommand(string src, string dest, DirectoryUtility.CopyProcessCallback callback)
            : base(String.Format("CopyDirectory {0}", src), true)
        {
            _src = src;
            _dest = dest;
            _callback = callback;
        } 
        #endregion

        #region Public Properties

        public RateStatistics CopySpeed
        {
            get { return _copySpeed; }
        }
        public TimeSpanStatistics BackupTime
        {
            get { return _backupTime; }
        } 
        #endregion

        #region Overridden Protected Methods

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            if (!Directory.Exists(_src))
                throw new DirectoryNotFoundException(string.Format("Source directory {0} does not exist", _src));

            if (RequiresRollback)
            {
                Backup();
            }


            CopySpeed.Start();
            _copied = true;
            ulong bytesCopied = DirectoryUtility.Copy(_src, _dest, _callback);
            CopySpeed.SetData(bytesCopied);
            CopySpeed.End();
        }


        protected override void OnUndo()
        {
            if (_copied)
            {
                if (Directory.Exists(_backupDestDir))
                {
                    try
                    {
                        DirectoryUtility.DeleteIfExists(_dest);
                    }
                    catch
                    {
                    	// ignore it, will overwrite anyway
                    }

                    // restore
                    try
                    {
                        DirectoryUtility.Copy(_backupDestDir, _dest);
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Warn, "Error occurred when rolling back CopyDirectoryCommand: {0}", ex);
                    }
                }
            }
        } 
        #endregion

        #region Private Members

        private void Backup()
        {
            if (Directory.Exists(_dest))
            {
                BackupTime.Start();
                var context = ProcessorContext as ServerExecutionContext;
                if (context != null)
                    _backupDestDir = Path.Combine(context.BackupDirectory, "DestFolder");
                Directory.CreateDirectory(_backupDestDir);
                Platform.Log(LogLevel.Info, "Backing up original destination folder {0}", _dest);
                DirectoryUtility.Copy(_dest, _backupDestDir);
                Platform.Log(LogLevel.Info, "Original destination folder {0} is backed up to {1}", _dest, _backupDestDir);
                BackupTime.End();
            }
        } 
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                DirectoryUtility.DeleteIfExists(_backupDestDir);
            }
            catch
            {
            	//ignore
            }
        }

        #endregion
    }
}