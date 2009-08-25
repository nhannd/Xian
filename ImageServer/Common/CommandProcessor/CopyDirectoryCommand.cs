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
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    public class CopyDirectoryCommand : ServerCommand, IDisposable
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

		protected override void OnExecute(ServerCommandProcessor theProcessor)
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
                _backupDestDir = Path.Combine(ExecutionContext.BackupDirectory, "DestFolder");
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