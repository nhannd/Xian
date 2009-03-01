#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
        private bool _renamed =false;
		#endregion

		public RenameFileCommand(string sourceFile, string destinationFile)
			: base(String.Format("Rename {0} to {1}", sourceFile, destinationFile), true)
		{
			Platform.CheckForNullReference(sourceFile, "Source filename");
			Platform.CheckForNullReference(destinationFile, "Destination filename");
		    Platform.CheckTrue(File.Exists(sourceFile), "Source file doesn't exist");

			_sourceFile = sourceFile;
			_destinationFile = destinationFile;
		}

		protected override void OnExecute()
		{
            if (RequiresRollback)
                Backup();

			if (File.Exists(_destinationFile))
			{
				File.Delete(_destinationFile);
			}

            File.Move(_sourceFile, _destinationFile);

		    SimulatePostOperationError();
		}

        [Conditional("DEBUG")]
        private void SimulatePostOperationError()
	    {
            ServerPlatform.SimulateError("Post File Rename Error", delegate() { File.Delete(_destinationFile); });
            ServerPlatform.SimulateError("Post File Rename Exception", delegate() { throw new Exception("Faked Exception"); });
	    }

	    private void Backup()
        {
            //backup source
            _srcBackupFile = String.Format("{0}.bak", _sourceFile);
            File.Copy(_sourceFile, _srcBackupFile, true);

            if (File.Exists(_destinationFile))
            {
                Random random = new Random();
                _destBackupFile = String.Format("{0}.bak", _destinationFile, random.Next());

                File.Copy(_destinationFile, _destBackupFile, true);
            }
        }

		protected override void OnUndo()
		{
            // restore the source
            if (false == String.IsNullOrEmpty(_srcBackupFile) && File.Exists(_srcBackupFile))
            {
                Platform.Log(LogLevel.Error, "Restoring {0}", _sourceFile);
                File.Copy(_srcBackupFile, _sourceFile);
            }

		    // restore destination
            if (false == String.IsNullOrEmpty(_destBackupFile) && File.Exists(_destBackupFile))
            {
                Platform.Log(LogLevel.Error, "Restoring {0}", _destinationFile);
                File.Copy(_destBackupFile, _destinationFile);
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