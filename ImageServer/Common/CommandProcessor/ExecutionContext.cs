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
using ClearCanvas.Enterprise.Core;
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
        protected string _backupDirectory;
        protected string _tempDirectory;
        private readonly object _sync = new object();
        private readonly ExecutionContext _inheritFrom;
        private IReadContext _readContext;
        private IPersistenceContext _persistenceContext;
        private readonly string _contextId;

        #endregion

        #region Constructors


        public ExecutionContext()
            : this(Guid.NewGuid().ToString())
        {

        }


        /// <summary>
        /// Creates an instance of <see cref="ExecutionContext"/> inherited
        /// the current ExecutionContext in the thread, if it exists. This context
        /// becomes the current context of the thread until it is disposed.
        /// </summary>
        public ExecutionContext(String contextId)
            : this(contextId, Current)
        {
         
        }

        public ExecutionContext(String contextId, ExecutionContext inheritFrom)
        {
            Platform.CheckForNullReference(contextId, "contextId");
            
            _contextId = contextId;
            _inheritFrom = inheritFrom;
            _current = this;

            if (inheritFrom!=null)
            {
                PersistenceContext = inheritFrom.PersistenceContext;
            }

        }

        #endregion

        #region Virtual Protected Methods

        protected virtual string GetTemporaryPath()
        {
            string path = _inheritFrom != null
                              ? Path.Combine(_inheritFrom.GetTemporaryPath(), _contextId)
                              : Path.Combine(ServerPlatform.TempDirectory, _contextId);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
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
                    _tempDirectory = GetTemporaryPath();

                return _tempDirectory;
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
                    }
                }

                if (!Directory.Exists(_backupDirectory))
                    Directory.CreateDirectory(_backupDirectory);

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

        public IPersistenceContext ReadContext
        {
            get
            {
                lock(SyncRoot)
                {
                    if (_readContext == null)
                        _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
                }

                return _readContext;
            }
        }

        public IPersistenceContext PersistenceContext
        {
            get
            {
                if (_persistenceContext != null)
                    return _persistenceContext;
                else
                    return ReadContext;
            }
            set
            {
                _persistenceContext = value;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (!DirectoryUtility.DeleteIfEmpty(_backupDirectory))
                {
                    Platform.Log(LogLevel.Warn, "Some backup files can be found left in {0}", BackupDirectory);
                }

                if (Platform.IsLogLevelEnabled(LogLevel.Debug) && Directory.Exists(_tempDirectory))
                    Platform.Log(LogLevel.Debug, "Deleting temp folder: {0}", _tempDirectory);
                
                DirectoryUtility.DeleteIfEmpty(_tempDirectory);
            }
            finally
            {
            	if (_readContext!=null)
            	{
            		_readContext.Dispose();
            		_readContext = null;
            	}
                // reset the current context for the thread
                _current = _inheritFrom;
            }
            
        }

        #endregion

        protected static string GetTempPathRoot()
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