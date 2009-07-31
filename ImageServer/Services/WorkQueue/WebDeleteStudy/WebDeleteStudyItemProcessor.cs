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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy
{
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    public class WebDeleteStudyItemProcessor : DeleteStudyItemProcessor
    {
        private DeletionLevel _level;

        public DeletionLevel Level
        {
            get { return _level; }
        }

        #region Overridden Protected Methods

        protected override void OnProcessItemBegin(Model.WorkQueue item)
        {
            base.OnProcessItemBegin(item);

            WebDeleteWorkQueueEntryData data = ParseQueueData(item);
            _level = data.Level;
        }

        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.Log(LogLevel.Info, "Begin {0} ({1} level) GUID={2}", item.WorkQueueTypeEnum, Level, item.Key);
            
            switch (Level)
            {
                case DeletionLevel.Series: ProcessSeriesLevelDelete(item);
                    break;
                case DeletionLevel.Study: ProcessStudyLevelDelete(item);
                    break;

                default:
                    throw new NotImplementedException();
            }

        }

        protected override bool CanStart()
        {
            if (!StorageLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.WebDeleteScheduled)
                && !LockStudyState(WorkQueueItem, QueueStudyStateEnum.WebDeleteScheduled))
            {
                PostponeItem(WorkQueueItem);
                return false;
            }

            return base.CanStart();
        } 
        #endregion

        #region Private Methods

        private static WebDeleteWorkQueueEntryData ParseQueueData(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item.Data, "item.Data");

            WebDeleteWorkQueueEntryData data = XmlUtils.Deserialize<WebDeleteWorkQueueEntryData>(item.Data);
            return data;
        }


        private void ProcessSeriesLevelDelete(Model.WorkQueue item)
        {
            // ensure the Study is loaded.
            Study study = StorageLocation.Study;
            Platform.CheckForNullReference(study, "Study record doesn't exist");

            Platform.Log(LogLevel.Info, "Processing Series Level Deletion for Study {1}, A#: {2}",
                                         study.StudyInstanceUid, study.AccessionNumber);

            bool completed = false;
            try
            {
                WebDeleteSeriesLevelQueueData queueData = XmlUtils.Deserialize<WebDeleteSeriesLevelQueueData>(item.Data);

                using (ServerCommandProcessor processor = new ServerCommandProcessor(String.Format("Deleting Series from study {0}, A#:{1}, Patient: {2}, ID:{3}",
                            study.StudyInstanceUid, study.AccessionNumber, study.PatientsName, study.PatientId)))
                {
                    StudyXml studyXml = StorageLocation.LoadStudyXml();
                    IList<Series> existingSeries = StorageLocation.Study.Series;

                    foreach (string seriesUid in queueData.SeriesInstanceUids)
                    {
                        Platform.Log(LogLevel.Info, "Deleting Series {0} from Study {1}, A#: {2}", seriesUid,
                                         study.StudyInstanceUid, study.AccessionNumber);

                        // Delete from study XML
                        if (studyXml.Contains(seriesUid))
                        {
                            //Note: DeleteDirectoryCommand  doesn't throw exception if the folder doesn't exist
                            RemoveSeriesFromStudyXml xmlUpdate = new RemoveSeriesFromStudyXml(studyXml, seriesUid);
                            processor.AddCommand(xmlUpdate);
                        }

                        // Delete from filesystem
                        string path = StorageLocation.GetSeriesPath(seriesUid);
                        if (Directory.Exists(path))
                        {
                            DeleteDirectoryCommand delDir = new DeleteDirectoryCommand(path, true);
                            processor.AddCommand(delDir);
                        }

                        // Delete from DB
                        bool existInDB = CollectionUtils.Contains(existingSeries, delegate(Series series) { return series.SeriesInstanceUid == seriesUid; });
                        if (existInDB)
                        {
                            DeleteSeriesFromDBCommand delSeries = new DeleteSeriesFromDBCommand(StorageLocation, seriesUid);
                            processor.AddCommand(delSeries);
                        }

                    }

                    if (!processor.Execute())
                        throw new ApplicationException(
                            String.Format("Error occurred when series from Study {0}, A#: {1}",
                                         study.StudyInstanceUid, study.AccessionNumber));
                    else
                    {
                        Platform.Log(LogLevel.Info, "Updating study xml...");
                        WriteStudyStream(StorageLocation.GetStudyXmlPath(), StorageLocation.GetCompressedStudyXmlPath(), studyXml);
                    }
                }


                completed = true;
            }
            finally
            {
                if (completed)
                {
                    PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                }
                else
                {
                    PostProcessing(item, WorkQueueProcessorStatus.Pending, WorkQueueProcessorDatabaseUpdate.None);
                }
            }
            
        }

        private void ProcessStudyLevelDelete(Model.WorkQueue item)
        {
            LoadExtensions();

            OnDeletingStudy();

            if (Study == null)
                Platform.Log(LogLevel.Info, "Deleting Study {0} on partition {1}",
                             StorageLocation.StudyInstanceUid, ServerPartition.AeTitle);
            else
                Platform.Log(LogLevel.Info,
                             "Deleting study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4}",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description);

            RemoveFilesystem();

            RemoveDatabase(item);

            OnStudyDeleted();

            if (Study == null)
                Platform.Log(LogLevel.Info, "Completed Deleting Study {0} on partition {1}",
                             StorageLocation.StudyInstanceUid, ServerPartition.AeTitle);
            else
                Platform.Log(LogLevel.Info,
                             "Completed Deleting study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4}",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description);
        }

        private static void WriteStudyStream(string streamFile, string gzStreamFile, StudyXml theStream)
        {
            XmlDocument doc = theStream.GetMemento(new StudyXmlOutputSettings());

            // allocate the random number generator here, in case we need it below
            Random rand = new Random();
            string tmpStreamFile = streamFile + "_tmp";
            string tmpGzStreamFile = gzStreamFile + "_tmp";
            for (int i = 0; ; i++)
                try
                {
                    if (File.Exists(tmpStreamFile))
                        FileUtils.Delete(tmpStreamFile);
                    if (File.Exists(tmpGzStreamFile))
                        FileUtils.Delete(tmpGzStreamFile);

                    using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(tmpStreamFile, FileMode.CreateNew),
                                      gzipStream = FileStreamOpener.OpenForSoleUpdate(tmpGzStreamFile, FileMode.CreateNew))
                    {
                        StudyXmlIo.WriteXmlAndGzip(doc, xmlStream, gzipStream);
                        xmlStream.Close();
                        gzipStream.Close();
                    }

                    if (File.Exists(streamFile))
                        FileUtils.Delete(streamFile);
                    File.Move(tmpStreamFile, streamFile);
                    if (File.Exists(gzStreamFile))
                        FileUtils.Delete(gzStreamFile);
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
