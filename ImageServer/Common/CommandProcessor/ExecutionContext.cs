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

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ExecutionContext"/> inherited
        /// the current ExecutionContext in the thread, if it exists. This context
        /// becomes the current context of the thread until it is disposed.
        /// </summary>
        public ExecutionContext()
            : this(Current)
        {
         
        }

        public ExecutionContext(ExecutionContext inheritFrom)
        {
            _inheritFrom = inheritFrom;
            _current = this;

            if (inheritFrom!=null)
            {
                TempDirectory = inheritFrom.TempDirectory;
                BackupDirectory = inheritFrom.BackupDirectory;
            }

        }

        #endregion

        #region Virtual Protected Methods
        protected virtual string GetTemporaryDirectory()
        {
            return ServerPlatform.TempDirectory;
        }
        #endregion

        protected object SyncRoot
        {
            get { return _sync; }
        }


        public String TempDirectory
        {
            get
            {
                if (_tempDirectory==null)
                {
                    lock(SyncRoot)
                    {
                        _tempDirectory = GetTemporaryDirectory();
                        if (!Directory.Exists(_tempDirectory))
                            Directory.CreateDirectory(_tempDirectory);
                    }
                }
               
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
                        DirectoryUtility.DeleteIfExists(_tempDirectory);
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
                        DirectoryUtility.DeleteIfExists(_tempDirectory);
                    }

                }
            }
            finally
            {
                // reset the current context for the thread
                _current = _inheritFrom;
            }
            
        }

        #endregion

        protected static string GetBaseTempPath()
        {
            String basePath = String.Empty;
            if (!String.IsNullOrEmpty(ImageServerCommonConfiguration.TemporaryPath))
            {
                if (Directory.Exists(ImageServerCommonConfiguration.TemporaryPath))
                    basePath = ImageServerCommonConfiguration.TemporaryPath;
                else
                {
                    // try to create it
                    try
                    {
                        Directory.CreateDirectory(ImageServerCommonConfiguration.TemporaryPath);
                    }
                    catch(Exception ex)
                    {
                        Platform.Log(LogLevel.Error, ex);
                    }
                }
            }
            return basePath;
        }
    }
}