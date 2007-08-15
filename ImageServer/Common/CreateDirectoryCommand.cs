using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// A ServerCommand derived class for creating a directory.
    /// </summary>
    public class CreateDirectoryCommand : ServerCommand
    {
        #region Private Members
        private string _directory;
        private bool _created = false;
        #endregion

        public CreateDirectoryCommand(string directory)
            : base("Create Directory")
        {
            Platform.CheckForNullReference(directory, "Directory name");

            _directory = directory;
        }

        public override void Execute()
        {
            if (Directory.Exists(_directory))
            {
                _created = false;
                return;
            }
            DirectoryInfo info = Directory.CreateDirectory(_directory);
            _created = true;
        }

        public override void Undo()
        {
            if (_created)
            {
                Directory.Delete(_directory);
                _created = false;
            }
        }
    }
}
