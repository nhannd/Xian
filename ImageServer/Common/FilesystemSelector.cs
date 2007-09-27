using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common
{
    public class FilesystemSelector
    {
        private FilesystemMonitor _monitor;

        public FilesystemSelector(FilesystemMonitor monitor)
        {
            _monitor = monitor;    
        }

        public Filesystem SelectFilesystem(DicomMessageBase msg)
        {
            ServerFilesystemInfo selectedFilesystem = null;
            float selectedFreeBytes = 0;

            foreach (ServerFilesystemInfo info in _monitor.Filesystems)
            {
                if (info.Online && info.Filesystem.Enabled && !info.Filesystem.ReadOnly)
                {
                    if (info.FreeBytes > selectedFreeBytes)
                    {
                        selectedFreeBytes = info.FreeBytes;
                        selectedFilesystem = info;
                    }
                }
            }

            if (selectedFilesystem == null)
                return null;

            return selectedFilesystem.Filesystem;
        }
    }
}
