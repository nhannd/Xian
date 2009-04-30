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
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
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
        private string _backupSrcDir ;
        private string _backupDestDir ;
        #endregion

        public MoveDirectoryCommand(string src, string dest)
            : base("MoveDirectory", true)
        {
            Platform.CheckForNullReference(src, "src");
            Platform.CheckForNullReference(dest, "dest");

            _src = src;
            _dest = dest;
        }

        protected override void OnExecute()
        {
            if (RequiresRollback)
            {
                Backup();
            }

            DirectoryUtility.Move(_src, _dest);
        }

        private void Backup()
        {
            // TODO: find a better way to backup the directory. For eg, will it be more efficient to backup the directory
            // by renaming it (if Windows API allow) or copy to a different location other than the current machine? 
            if (Directory.Exists(_src))
            {
                _backupSrcDir = ExecutionContext.TempDirectory;

                Platform.Log(LogLevel.Debug, "Backing up original source folder {0}", _src);
                DirectoryUtility.Copy(_src, _backupSrcDir);

                Platform.Log(LogLevel.Info, "Original source folder {0} is backed up to {1}", _src, _backupSrcDir);
            }


            if (Directory.Exists(_dest))
            {
                _backupDestDir = ExecutionContext.TempDirectory;
                Platform.Log(LogLevel.Debug, "Backing up original destination folder {0}", _dest);
                DirectoryUtility.Copy(_dest, _backupDestDir);
                Platform.Log(LogLevel.Info, "Original destination folder {0} is backed up to {1}", _dest, _backupDestDir);
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
                if (Directory.Exists(_backupSrcDir))
                {
                    Platform.Log(LogLevel.Info, "Restoring original source folder {0}", _src);
                    DirectoryUtility.Copy(_backupSrcDir, _src);
                }

                if (Directory.Exists(_backupDestDir))
                {
                    Platform.Log(LogLevel.Info, "Restoring original destination folder {0}", _dest);
                    DirectoryUtility.Copy(_backupDestDir, _dest);
                }

            }

        }

        #region IDisposable Members

        public void Dispose()
        {
            // Backup directories are created inside the context temporary folder.
            // They will be deleted automatically
        }

        #endregion
    }
}
