#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    internal class RemoveInstanceFromStudyXmlCommand : ServerCommand
    {
        #region Private Members

        private readonly StudyStorageLocation _studyLocation;
        private readonly DicomFile _file;
        private readonly StudyXml _studyXml;

        #endregion

        #region Constructors

        public RemoveInstanceFromStudyXmlCommand(StudyStorageLocation location, StudyXml studyXml, DicomFile file)
            :base("Remove Instance From Study Xml", true)
        {
            _studyLocation = location;
            _file = file;
            _studyXml = studyXml;
        }

        #endregion

        #region Overridden Protected Methods

        protected override void OnExecute(ServerCommandProcessor theProcessor)
        {
            _studyXml.RemoveFile(_file);

            // flush it into disk
            // Write it back out.  We flush it out with every added image so that if a failure happens,
            // we can recover properly.
            string streamFile = _studyLocation.GetStudyXmlPath();
            string gzStreamFile = streamFile + ".gz";

            WriteStudyStream(streamFile, gzStreamFile, _studyXml);
            
        }

        protected override void OnUndo()
        {
            _studyXml.AddFile(_file);

            string streamFile = _studyLocation.GetStudyXmlPath();
            string gzStreamFile = streamFile + ".gz";
            WriteStudyStream(streamFile, gzStreamFile, _studyXml);
        }

        #endregion

        #region Private Methods

        private static void WriteStudyStream(string streamFile, string gzStreamFile, StudyXml theStream)
        {
            XmlDocument doc = theStream.GetMemento(ImageServerCommonConfiguration.DefaultStudyXmlOutputSettings);

            // allocate the random number generator here, in case we need it below
            Random rand = new Random();
            string tmpStreamFile = streamFile + "_tmp";
            string tmpGzStreamFile = gzStreamFile + "_tmp";
            for (int i = 0; ; i++)
                try
                {
                    if (File.Exists(tmpStreamFile))
                        File.Delete(tmpStreamFile);
                    if (File.Exists(tmpGzStreamFile))
                        File.Delete(tmpGzStreamFile);

                    using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(tmpStreamFile, FileMode.CreateNew),
                                      gzipStream = FileStreamOpener.OpenForSoleUpdate(tmpGzStreamFile, FileMode.CreateNew))
                    {
                        StudyXmlIo.WriteXmlAndGzip(doc, xmlStream, gzipStream);
                        xmlStream.Close();
                        gzipStream.Close();
                    }

                    if (File.Exists(streamFile))
                        File.Delete(streamFile);
                    File.Move(tmpStreamFile, streamFile);
                    if (File.Exists(gzStreamFile))
                        File.Delete(gzStreamFile);
                    File.Move(tmpGzStreamFile, gzStreamFile);
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

        #endregion

    }
}