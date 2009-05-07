using System;
using System.IO;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{
    internal class DirectoryImporterParameters
    {
        public String PartitionAE;
        public DirectoryInfo Directory;
        public int MaxImages;
        public int Delay;
        public string Filter;
    }
}