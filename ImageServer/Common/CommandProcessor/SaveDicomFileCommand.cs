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

using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	public class SaveDicomFileCommand : ServerCommand
	{
		#region Private Members
		private readonly string _path;
		private readonly DicomFile _file;
	    private string _backupPath = ServerPlatform.GetTempPath();
	    private bool _backup;
	    private bool _saved;
		#endregion

		public SaveDicomFileCommand(string path, DicomFile file )
			: base("Save DICOM Message", true)
		{
			Platform.CheckForNullReference(path, "File name");
			Platform.CheckForNullReference(file, "Dicom File object");

            if (RequiresRollback)
                Backup();

			_path = path;
			_file = file;
		}

	    private void Backup()
	    {
            if (File.Exists(_path))
            {
                File.Copy(_path, _backupPath);
                _backup = true;
            }
	    }


	    protected override void OnExecute()
		{
            _file.Save(_path, DicomWriteOptions.Default);
	        _saved = true;
		}

		protected override void OnUndo()
		{
            if (_backup)
            {
                if (_saved)
                    File.Copy(_backupPath, _path);
            }
            else
            {
                File.Delete(_path);
            }
		}
	}
}