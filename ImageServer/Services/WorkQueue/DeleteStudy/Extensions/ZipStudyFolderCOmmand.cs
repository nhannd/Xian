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
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using Ionic.Zip;

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions
{
    internal class ZipStudyFolderCommand : ServerCommand, IDisposable
    {
        private readonly string _source;
        private readonly string _dest;
        private string _destBackup;

        public ZipStudyFolderCommand(string source, string dest)
            : base("Zip study folder", true)
        {
            Platform.CheckForNullReference(source, "source");
            Platform.CheckForNullReference(dest, "dest");
            _source = source;
            _dest = dest;
        }

		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			if (RequiresRollback)
			{
				Backup();
			}

			using (ZipFile zip = new ZipFile(_dest))
			{
				zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
				zip.Comment = String.Format("Archive for deleted study from path {0}", _source);
	
				zip.AddDirectory(_source, String.Empty);
				zip.Save();
			}
		}

    	private void Backup()
        {
            if (File.Exists(_dest))
            {
                _destBackup = _dest + ".bak";
                if (File.Exists(_destBackup))
                    FileUtils.Delete(_destBackup);

                FileUtils.Copy(_dest, _destBackup, true);
            }
        }

        protected override void OnUndo()
        {
            if (File.Exists(_dest))
            {
				FileUtils.Delete(_dest);
            }

            // restore backup
            if (File.Exists(_destBackup))
            {
                File.Move(_destBackup, _dest);
            }
            
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (File.Exists(_destBackup))
            {
                FileUtils.Delete(_destBackup);
            }
            
        }

        #endregion
    }
}