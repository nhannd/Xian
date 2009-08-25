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
    /// <summary>
    /// A ServerCommand derived class for moving a directory.
    /// </summary>
    /// <remark>
    /// If <see cref="ServerCommand.RequiresRollback"/> is set to <b>true</b>, the <see cref="MoveDirectoryCommand"/>
    /// will perform necessary backup so that the original source and destination directories can be restored when <see cref="OnUndo"/> is called.
    /// </remark>
    public class MoveDirectoryCommand : ServerCommand, IDisposable
    {
        #region Private Members
        private readonly string _src;
        private readonly string _dest;
        private string _backupDestDir ;
        private readonly TimeSpanStatistics _backupTime = new TimeSpanStatistics();
        private readonly RateStatistics _moveSpeed = new RateStatistics("MoveSpeed", RateType.BYTES);
        private DirectoryUtility.CopyProcessCallback _callback;
        #endregion

        public MoveDirectoryCommand(string src, string dest, DirectoryUtility.CopyProcessCallback callback)
            : base(String.Format("MoveDirectory {0}", src), true)
        {
            Platform.CheckForNullReference(src, "src");
            Platform.CheckForNullReference(dest, "dest");

            _src = src;
            _dest = dest;
            _callback = callback;
        }

        public TimeSpanStatistics BackupTime
        {
            get { return _backupTime; }
        }

        public RateStatistics MoveSpeed
        {
            get { return _moveSpeed; }
        }

		protected override void OnExecute(ServerCommandProcessor theProcessor)
        {
            if (!Directory.Exists(_src))
                throw new DirectoryNotFoundException(string.Format("Source directory {0} does not exist", _src));
            
            if (RequiresRollback)
            {
                Backup();
            }



            MoveSpeed.Start();
            ulong bytesCopied = DirectoryUtility.Copy(_src, _dest, _callback);
            MoveSpeed.SetData(bytesCopied);
            MoveSpeed.End();
        }

        private void Backup()
        {
            if (Directory.Exists(_dest))
            {
                BackupTime.Start();
                _backupDestDir = Path.Combine(ExecutionContext.BackupDirectory, "DestStudy");
                Directory.CreateDirectory(_backupDestDir);
                Platform.Log(LogLevel.Info, "Backing up original destination folder {0}", _dest);
                DirectoryUtility.Copy(_dest, _backupDestDir);
                Platform.Log(LogLevel.Info, "Original destination folder {0} is backed up to {1}", _dest, _backupDestDir);
                BackupTime.End();
            }

        }

        protected override void OnUndo()
        {
            try
            {
                DirectoryUtility.DeleteIfExists(_dest, true);
            }
            finally
            {
                if (Directory.Exists(_backupDestDir))
                {
                    Platform.Log(LogLevel.Info, "Restoring original destination folder {0}", _dest);
                    DirectoryUtility.Copy(_backupDestDir, _dest);
                    Platform.Log(LogLevel.Info, "Original destination folder {0} is restored", _dest);
                }
            }

        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!RollBackRequested)
            {
                DirectoryUtility.DeleteIfExists(_src);
            }

            DirectoryUtility.DeleteIfExists(_backupDestDir);
        }

        #endregion
    }
}
