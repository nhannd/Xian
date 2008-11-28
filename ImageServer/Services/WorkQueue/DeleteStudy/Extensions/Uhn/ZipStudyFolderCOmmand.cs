using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using Ionic.Utils.Zip;

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions.Uhn
{
    internal class ZipStudyFolderCommand : ServerCommand, IDisposable
    {
        private readonly string _source;
        private readonly string _dest;
        private string _destBackup;

        public ZipStudyFolderCommand(string source, string dest)
            : base("Zip study folder", true)
        {
            _source = source;
            _dest = dest;
        }

        protected override void OnExecute()
        {
            if (RequiresRollback)
            {
                Backup();
            }


            using (FileStream output = File.Create(_dest))
            {
                using (ZipFile zip = new ZipFile(output))
                {
                    zip.AddDirectory(_source, String.Empty);
                    zip.Save();
                }
            }


        }

        private void Backup()
        {
            if (File.Exists(_dest))
            {
                _destBackup = Path.Combine(_dest, ".bak");
                File.Copy(_dest, _destBackup, true);
            }
        }

        protected override void OnUndo()
        {
            if (File.Exists(_dest))
            {
                File.Delete(_dest);
            }

            // restore backup
            if (String.IsNullOrEmpty(_destBackup) == false)
            {
                if (File.Exists(_destBackup))
                {
                    File.Move(_destBackup, _dest);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (String.IsNullOrEmpty(_destBackup))
            {
                if (File.Exists(_destBackup))
                {
                    File.Delete(_destBackup);
                }
            }
        }

        #endregion
    }

}
