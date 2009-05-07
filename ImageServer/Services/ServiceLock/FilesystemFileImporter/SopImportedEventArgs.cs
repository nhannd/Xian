using System;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{
    internal class SopImportedEventArgs : EventArgs
    {
        public string  StudyInstanceUid;
        public string  SeriesInstanceUid;
        public string  SopInstanceUid;
    }
}