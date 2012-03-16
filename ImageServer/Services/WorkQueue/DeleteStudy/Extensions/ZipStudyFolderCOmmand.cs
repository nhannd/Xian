#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common.Utilities;
using Ionic.Zip;

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions
{
    internal class ZipStudyFolderCommand : CommandBase, IDisposable
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

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			if (RequiresRollback)
			{
				Backup();
			}

			using (var zip = new ZipFile(_dest))
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