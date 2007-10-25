#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.DicomServices.Xml;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    public class InsertStreamCommand : ServerCommand
    {
        #region Private Members

        private DicomFile _file;
        private StudyXml _stream;
        private string _studyStreamPath;
        #endregion

        public InsertStreamCommand( DicomFile file, StudyXml stream, string studyStreamPath)
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

        private static void WriteStudyStream(string streamFile, StudyXml theStream)
        {
            Stream fileStream = null;

            try
            {
                XmlDocument doc = theStream.GetMemento();
                if (File.Exists(streamFile))
                    File.Delete(streamFile);

                fileStream = new FileStream(streamFile, FileMode.CreateNew);

                StudyXmlIo.Write(doc, fileStream);

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