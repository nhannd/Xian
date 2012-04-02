using System;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core.Command
{
    public class InsertStudyXmlCommand : CommandBase
    {
        #region Private Members

        private readonly DicomFile _file;
        private readonly StudyXml _studyXml;
        private readonly StudyLocation _studyStorageLocation;
        private readonly bool _writeFile;
        private readonly StudyXmlOutputSettings _settings;
        #endregion

        public InsertStudyXmlCommand(DicomFile file, StudyXml studyXml, StudyLocation storageLocation, bool writeFile)
            : base("Insert Study Xml", true)
        {
            _file = file;
            _studyXml = studyXml;
            _studyStorageLocation = storageLocation;
            _writeFile = writeFile;

            _settings = new StudyXmlOutputSettings
                            {
                                IncludePrivateValues = StudyXmlTagInclusion.IgnoreTag,
                                IncludeUnknownTags = StudyXmlTagInclusion.IgnoreTag,
                                IncludeLargeTags = StudyXmlTagInclusion.IncludeTagExclusion,
                                MaxTagLength = 2048,
                                IncludeSourceFileName = true
                            };
        }

        private void WriteStudyStream(string streamFile, StudyXml theStream)
        {
            var doc = theStream.GetMemento(_settings);

            // allocate the random number generator here, in case we need it below
            var rand = new Random();
            string tmpStreamFile = streamFile + "_tmp";
            for (int i = 0; ; i++)
                try
                {
                    if (File.Exists(tmpStreamFile))
                        FileUtils.Delete(tmpStreamFile);

                    using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(tmpStreamFile, FileMode.CreateNew))
                    {
                        StudyXmlIo.Write(doc, xmlStream);
                        xmlStream.Close();
                    }

                    File.Copy(tmpStreamFile, streamFile, true);
                    FileUtils.Delete(tmpStreamFile);
           
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

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            long fileSize = 0;
            if (File.Exists(_file.Filename))
            {
                var finfo = new FileInfo(_file.Filename);
                fileSize = finfo.Length;
            }

            // Setup the insert parameters
            if (false == _studyXml.AddFile(_file, fileSize, _settings))
            {
                Platform.Log(LogLevel.Error, "Unexpected error adding SOP to XML Study Descriptor for file {0}",
                             _file.Filename);
                throw new ApplicationException("Unexpected error adding SOP to XML Study Descriptor for SOP: " +
                                               _file.MediaStorageSopInstanceUid);
            }

            if (_writeFile)
            {
                // Write it back out.  We flush it out with every added image so that if a failure happens,
                // we can recover properly.

                WriteStudyStream(_studyStorageLocation.GetStudyXmlPath(), _studyXml);
            }
        }

        protected override void OnUndo()
        {
            Platform.Log(LogLevel.Info, "Undoing InsertStudyXmlCommand");
            _studyXml.RemoveFile(_file);
            
            if (_writeFile)
            {
                WriteStudyStream(_studyStorageLocation.GetStudyXmlPath(), _studyXml);
            }
        }
    }
}
