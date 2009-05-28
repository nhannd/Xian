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

using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using System;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// A ServerCommand derived class for renaming a file.
	/// </summary>
	public class RenameFileCommand : ServerCommand, IDisposable
	{
		#region Private Members
		private readonly string _sourceFile;
		private readonly string _destinationFile;
        private string _srcBackupFile; 
        private string _destBackupFile;
	    private readonly bool _failIfExists;
	    private bool _sourceRenamed;

	    #endregion

		public RenameFileCommand(string sourceFile, string destinationFile, bool failIfExists)
			: base(String.Format("Rename {0} to {1}", sourceFile, destinationFile), true)
		{
			Platform.CheckForNullReference(sourceFile, "Source filename");
			Platform.CheckForNullReference(destinationFile, "Destination filename");
		    
			_sourceFile = sourceFile;
			_destinationFile = destinationFile;
		    _failIfExists = failIfExists;
		}

		protected override void OnExecute()
		{
            Platform.CheckTrue(File.Exists(_sourceFile), String.Format("Source file '{0}' doesn't exist", _sourceFile));
            
            if (File.Exists(_destinationFile))
            {
                if (_failIfExists)
                    throw new ApplicationException(String.Format("Destination file already exists: {0}", _destinationFile));
            }

            if (RequiresRollback)
                Backup();

            if (File.Exists(_destinationFile)) 
                FileUtils.Delete(_destinationFile);

            File.Move(_sourceFile, _destinationFile);
		    _sourceRenamed = true;

		    SimulatePostOperationError();
		}

        [Conditional("DEBUG")]
        private void SimulatePostOperationError()
	    {
            Diagnostics.RandomError.Generate(Diagnostics.Settings.SimulateFileIOError, "Post File Rename Error", delegate { File.Delete(_destinationFile); });
	    }

	    private void Backup()
        {
			//backup source
			_srcBackupFile = FileUtils.Backup(_sourceFile);

            if (File.Exists(_destinationFile))
            {
				_destBackupFile = FileUtils.Backup(_sourceFile);
            }
        }

		protected override void OnUndo()
		{
            // restore the source
            if (File.Exists(_srcBackupFile))
            {
                if (_sourceRenamed)
                {
                    try
                    {
                        Platform.Log(LogLevel.Info, "Restoring {0}", _sourceFile);
                        FileUtils.Copy(_srcBackupFile, _sourceFile, true);
                    }
                    catch(Exception e)
                    {
                        Platform.Log(LogLevel.Error, "Error occured when rolling back source file in RenameFileCommand: {0}", e.Message);
                    }
                }
            }

		    // restore destination
            if (File.Exists(_destBackupFile))
            {
                try
                {
                    Platform.Log(LogLevel.Error, "Restoring {0}", _destinationFile);
                    FileUtils.Copy(_destBackupFile, _destinationFile, true);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, "Error occured when rolling back destination file in RenameFileCommand: {0}", e.Message);
                } 
            }
			
		}

        #region IDisposable Members

        public void Dispose()
        {
            if (File.Exists(_srcBackupFile))
                File.Delete(_srcBackupFile); 
            
            if (File.Exists(_destBackupFile))
                File.Delete(_destBackupFile);

        }

        #endregion
    }
}