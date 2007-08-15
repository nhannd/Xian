using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Dicom;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
    public class SaveDicomFileCommand : ServerCommand
    {
        #region Private Members
        private string _path;
        private DicomFile _file;
        #endregion

        public SaveDicomFileCommand(string path, DicomFile file )
            : base("Save DICOM Message")
        {
            Platform.CheckForNullReference(path, "File name");
            Platform.CheckForNullReference(file, "Dicom File object");

            _path = path;
            _file = file;
        }

        public override void Execute()
        {
            _file.Save(DicomWriteOptions.Default);
        }

        public override void Undo()
        {
            File.Delete(_path);
        }
    }
}
