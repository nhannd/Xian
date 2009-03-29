using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    /// <summary>
    /// Represents the execution context of the current operation.
    /// </summary>
    public class ExecutionContext: IDisposable
    {
        #region Private Fields
        [ThreadStatic]
        private static ExecutionContext _current;
        protected string _tempDirectory;
        protected string _backupDirectory;
        private readonly object _sync = new object();
        private readonly ExecutionContext _inheritFrom;

        #endregion

        public ExecutionContext()
        {
            _inheritFrom = null;
            _current = this;
            TempDirectory = ServerPlatform.TempDirectory;
        }


        public ExecutionContext(ExecutionContext inheritFrom)
        {
            Platform.CheckForNullReference(inheritFrom, "inheritFrom");
            _inheritFrom = inheritFrom;
            _current = this;

            TempDirectory = inheritFrom.TempDirectory;
            BackupDirectory = inheritFrom.BackupDirectory;
        }

        public String TempDirectory
        {
            get
            {
                return _tempDirectory;
            }
            set
            {
                _tempDirectory = value;
            }
        }

        public virtual String BackupDirectory
        {
            get
            {
                if (_backupDirectory == null)
                {
                    lock (SyncRoot)
                    {
                        String baseBackupDirectory = Path.Combine(TempDirectory, "BackupFiles");
                        _backupDirectory = baseBackupDirectory;
                        Directory.CreateDirectory(_backupDirectory);
                    }
                }

                return _backupDirectory;
            }
            set
            {
                _backupDirectory = value;
            }
        }

        public static ExecutionContext Current
        {
            get { return _current; }
        }

        protected object SyncRoot
        {
            get { return _sync; }
        }


        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_backupDirectory))
                {
                    // If the backupdir is empty, that means everyone has claimed its stuff.
                    // We can remove the entire temp folder.
                    if (DirectoryUtility.DeleteIfEmpty(_backupDirectory))
                    {
                        DirectoryUtility.DeleteIfExists(_tempDirectory, true);
                    }
                    else
                    {
                        Platform.Log(LogLevel.Warn, "Some backup files can be found left in {0}", BackupDirectory);
                    }
                }
                else
                {
                    if (Directory.Exists(_tempDirectory))
                    {
                        // Nobody asked for backup storage or all of them have claimed the stuff. 
                        // We can remove the entire temp folder.
                        DirectoryUtility.DeleteIfExists(_tempDirectory, true);
                    }

                }
            }
            finally
            {
                _current = _inheritFrom;
            }
            
        }

        #endregion
    }
}