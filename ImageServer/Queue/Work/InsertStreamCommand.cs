using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Streaming;

namespace ClearCanvas.ImageServer.Queue.Work
{
    public class InsertStreamCommand : ServerCommand
    {
        #region Private Members

        private DicomFile _file;
        private StudyStream _stream;
        #endregion

        public InsertStreamCommand( DicomFile file, StudyStream stream)
            : base("Insert Instance into Database")
        {
            Platform.CheckForNullReference(file, "Dicom File object");
            Platform.CheckForNullReference(stream, "StudyStream object");

            _file = file;
            _stream = stream;
            
        }

        public override void Execute()
        {
            // Setup the insert parameters
            if (false == _stream.AddFile(_file))
            {
                Platform.Log(LogLevel.Error,"Unexpected error adding SOP to XML Study Descriptor for file {0}",_file);
            }
           
        }

        public override void Undo()
        {

        }
    }
}
