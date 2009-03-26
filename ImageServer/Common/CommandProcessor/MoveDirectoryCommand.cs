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
        private bool _backedup = true;
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
                _backupSrcDir = ExecutionContext.GetUniqueTempDir();

                Platform.Log(LogLevel.Debug, "Backing up original source folder {0}", _src);
                DirectoryUtility.Copy(_dest, _backupSrcDir);

                Platform.Log(LogLevel.Info, "Original source folder {0} is backed up to {1}", _src, _backupSrcDir);
            }


            if (Directory.Exists(_dest))
            {
                _backupDestDir = ExecutionContext.GetUniqueTempDir();
                Platform.Log(LogLevel.Debug, "Backing up original destination folder {0}", _dest);
                DirectoryUtility.Copy(_dest, _backupDestDir);
                Platform.Log(LogLevel.Info, "Original destination folder {0} is backed up to {1}", _dest, _backupDestDir);
            }

            _backedup = true;

        }

        protected override void OnUndo()
        {
            try
            {
                DirectoryUtility.DeleteIfExists(_dest, true);
            }
            finally
            {
                if (_backedup)
                {
                    Platform.Log(LogLevel.Info, "Restoring original source folder {0}", _src);
                    DirectoryUtility.Copy(_backupSrcDir, _src);

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
