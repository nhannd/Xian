#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;


namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    public class DeleteDirectoryCommand : ServerCommand, IDisposable
    {
        #region Private Members
        private readonly string _dir;
        private readonly bool _failIfError;
        private bool _sourceDirRenamed;
        private readonly TimeSpanStatistics _deleteTime = new TimeSpanStatistics("DeleteDirectoryTime");
        #endregion

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

		protected override void OnExecute(ServerCommandProcessor theProcessor)
        {
            try
            {
                if (Directory.Exists(_dir))
                {
                    if (DeleteOnlyIfEmpty && !DirectoryUtility.IsEmpty(_dir))
                    {
                        return;
                    }

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
