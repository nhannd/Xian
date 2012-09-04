#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Threading;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    internal class RemoveInstanceFromStudyXmlCommand : CommandBase
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

        protected override void OnExecute(CommandProcessor commandProcessor)
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