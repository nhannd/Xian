#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Threading;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    public class UpdateStudyXmlCommand : ServerCommand
    {
        #region Private Members

        private readonly DicomFile _file;
        private readonly StudyXml _stream;
        private readonly StudyStorageLocation _studyStorageLocation;

        #endregion

        #region Constructors

        public UpdateStudyXmlCommand(DicomFile file, StudyXml stream, StudyStorageLocation storageLocation)
            : base("Update Study XML", true)
        {
            Platform.CheckForNullReference(file, "Dicom File object");
            Platform.CheckForNullReference(stream, "StudyStream object");
            Platform.CheckForNullReference(storageLocation, "Study Storage Location");

            _file = file;
            _stream = stream;
            _studyStorageLocation = storageLocation;
        }

        #endregion

        protected override void OnExecute()
        {
            // Setup the insert parameters
            if (false == _stream.RemoveFile(_file))
            {
                Platform.Log(LogLevel.Warn, "SOP was unexpectedly not in XML Study Descriptor for file: {0}",
                             _file.Filename);
            }
            if (false == _stream.AddFile(_file))
            {
                Platform.Log(LogLevel.Error, "Unexpected error adding SOP to XML Study Descriptor for file {0}",
                             _file.Filename);
                throw new ApplicationException("Unexpected error adding SOP to XML Study Descriptor for SOP: " +
                                               _file.MediaStorageSopInstanceUid);
            }
            // Write it back out.  We flush it out with every added image so that if a failure happens,
            // we can recover properly.
            WriteStudyStream(
				Path.Combine(_studyStorageLocation.GetStudyPath(), _studyStorageLocation.StudyInstanceUid + ".xml"),
				Path.Combine(_studyStorageLocation.GetStudyPath(), _studyStorageLocation.StudyInstanceUid + ".xml.gz"),
				_stream);
        }

        protected override void OnUndo()
        {
            _stream.AddFile(_file);

            WriteStudyStream(
                Path.Combine(_studyStorageLocation.GetStudyPath(), _studyStorageLocation.StudyInstanceUid + ".xml"),
				Path.Combine(_studyStorageLocation.GetStudyPath(), _studyStorageLocation.StudyInstanceUid + ".xml.gz"),
                _stream);
        }

        private static void WriteStudyStream(string streamFile, string gzStreamFile, StudyXml theStream)
        {
            XmlDocument doc = theStream.GetMemento(ImageServerCommonConfiguration.DefaultStudyXmlOutputSettings);

            // allocate the random number generator here, in case we need it below
            Random rand = new Random();

            for (int i = 0;; i++)
                try
                {
                    if (File.Exists(streamFile))
                        File.Delete(streamFile);
					if (File.Exists(gzStreamFile))
						File.Delete(gzStreamFile);

					using (FileStream xmlStream = new FileStream(streamFile, FileMode.CreateNew),
									  gzipStream = new FileStream(gzStreamFile, FileMode.CreateNew))
					{
						StudyXmlIo.WriteXmlAndGzip(doc, xmlStream, gzipStream);
						xmlStream.Close();
						gzipStream.Close();
					}
                	return;
                }
                catch (IOException)
                {
                    if (i < 5)
                    {
                        Thread.Sleep(rand.Next(5, 50)); // Sleep 5-50 milliseconds
                        continue;
                    }

                    throw;
                }
        }
    }
}