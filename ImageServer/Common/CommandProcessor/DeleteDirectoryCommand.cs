using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    /// <summary>
    /// A ServerCommand derived class for deleting a directory.
    /// </summary>
    public class DeleteDirectoryCommand : ServerCommand, IDisposable
    {
        #region Private Members
        private readonly string _directory;
        private readonly string _backupDirectory = ServerPlatform.GetTempPath();
        private bool _backedUp = false;
        private readonly bool _deleteWhenEmpty = false;
        #endregion

        public DeleteDirectoryCommand(string directory, bool deleteWhenEmpty)
            : base("Delete Directory", true)
        {
            Platform.CheckForNullReference(directory, "Directory name");
            _directory = directory;
            _deleteWhenEmpty = deleteWhenEmpty;
        }

        protected override void OnExecute()
        {
            if (Directory.Exists(_directory))
            {
                if (RequiresRollback)
                {
                    DirectoryUtility.Move(_directory, _backupDirectory);
                    _backedUp = true;
                }

                if (_deleteWhenEmpty)
                    DirectoryUtility.DeleteIfEmpty(_directory);
                else
                    DirectoryUtility.DeleteIfExists(_directory);
            }
        }

        protected override void OnUndo()
        {
            if (_backedUp)
            {
                DirectoryUtility.DeleteIfExists(_directory);
                DirectoryUtility.Move(_backupDirectory, _directory);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            DirectoryUtility.DeleteIfExists(_backupDirectory);
        }

        #endregion
    }
}
