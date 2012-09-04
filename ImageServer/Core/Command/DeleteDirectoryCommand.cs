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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Command;

namespace ClearCanvas.ImageServer.Core.Command
{
    public class DeleteDirectoryCommand : CommandBase, IDisposable
    {
        #region Private Members
        private readonly string _dir;
        private readonly bool _failIfError;
        private bool _sourceDirRenamed;
        private readonly TimeSpanStatistics _deleteTime = new TimeSpanStatistics("DeleteDirectoryTime");
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether a log will be 
        /// generated when the specified directory is deleted.
        /// </summary>
        public bool Log { get; set; }

        #region Constructors

        public DeleteDirectoryCommand(string dir, bool failIfError)
            : base(String.Format("DeleteDirectory {0}", dir), true)
        {
            _dir = dir;
            _failIfError = failIfError;
        }

		public DeleteDirectoryCommand(string dir, bool failIfError, bool deleteOnlyIfEmpty)
			: base(String.Format("DeleteDirectory {0}", dir), true)
		{
			_dir = dir;
			_failIfError = failIfError;
			DeleteOnlyIfEmpty = deleteOnlyIfEmpty;
		}

		/// <summary>
		/// Gets the time spent on deleting the directory.
		/// </summary>
        public TimeSpanStatistics DeleteTime
        {
            get { return _deleteTime; }
        }

        public bool DeleteOnlyIfEmpty { get; set; }

        #endregion

        #region Overridden Protected Methods

		protected override void OnExecute(CommandProcessor theProcessor)
        {
            try
            {
                if (Directory.Exists(_dir))
                {
                    if (DeleteOnlyIfEmpty && !DirectoryUtility.IsEmpty(_dir))
                    {
                        return;
                    }

                    if (Log)
                        Platform.Log(LogLevel.Info, "Deleting {0}", _dir);

                    Directory.Move(_dir, _dir +".deleted");
                    _sourceDirRenamed = true;
                }
                
            }
            catch (Exception ex)
            {
                if (_failIfError)
                    throw;
            	// ignore it
            	Platform.Log(LogLevel.Warn, ex, "Unexpected exception occurred when deleting {0}. It is ignored.", _dir);
            }
        }

        protected override void OnUndo()
        {
            // the directory has been backed up.. it can be restored
            try
            {
                if (_sourceDirRenamed)
                {
                    Directory.Move(_dir + ".deleted", _dir);
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Error occurred while restoring {0}", _dir);
                throw;
            }
            
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        	// If not rolling back and the dir was renamed by this command 
        	// then delete it. Otherwise, just leave the ".deleted" directory
            if (!RollBackRequested && _sourceDirRenamed)
            {
                DeleteTime.Start();
                DirectoryUtility.DeleteIfExists(_dir + ".deleted"); 
                DeleteTime.End();
            }
        }

        #endregion
    }
}
