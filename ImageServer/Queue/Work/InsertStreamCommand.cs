using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
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
        private string _studyStreamPath;
        #endregion

        public InsertStreamCommand( DicomFile file, StudyStream stream, string studyStreamPath)
            : base("Insert Instance into Database")
        {
            Platform.CheckForNullReference(file, "Dicom File object");
            Platform.CheckForNullReference(stream, "StudyStream object");
            Platform.CheckForNullReference(studyStreamPath, "Path to Stream XML file");

            _file = file;
            _stream = stream;
            _studyStreamPath = studyStreamPath;
            
        }

        public override void Execute()
        {
            // Setup the insert parameters
            if (false == _stream.AddFile(_file))
            {
                Platform.Log(LogLevel.Error,"Unexpected error adding SOP to XML Study Descriptor for file {0}",_file.Filename);
                throw new ApplicationException("Unexpected error adding SOP to XML Study Descriptor for SOP: " + _file.MediaStorageSopInstanceUid);
            }
            // Write it back out.  We flush it out with every added image so that if a failure happens,
            // we can recover properly.
            WriteStudyStream(_studyStreamPath, _stream);
        }

        public override void Undo()
        {
            _stream.RemoveFile(_file);

            WriteStudyStream(_studyStreamPath, _stream);
        }

        private static void WriteStudyStream(string streamFile, StudyStream theStream)
        {
            Stream fileStream = null;

            try
            {
                XmlDocument doc = theStream.GetMomento();

                if (File.Exists(streamFile))
                    File.Delete(streamFile);

                fileStream = new FileStream(streamFile, FileMode.CreateNew);

                StreamingIo.Write(doc, fileStream);

            }
            finally
            {
                // We have to throw the exception here to cleanup 
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }
    }
}
