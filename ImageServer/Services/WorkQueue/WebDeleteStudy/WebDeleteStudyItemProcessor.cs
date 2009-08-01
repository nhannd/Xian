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
using ClearCanvas.Enterprise.Core;
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
        private string _reason;
        private string _userId;
        private IList<IWebDeleteProcessorExtension> _extensions;
        private List<Series> _seriesToDelete;

        public DeletionLevel Level
        {
            get { return _level; }
        }

        public string Reason
        {
            get { return _reason; }
        }

        public string UserID
        {
            get { return _userId; }
        }

        #region Overridden Protected Methods

        protected override void OnProcessItemBegin(Model.WorkQueue item)
        {
            base.OnProcessItemBegin(item);

            WebDeleteWorkQueueEntryData data = ParseQueueData(item);
            _level = data.Level;
            _reason = data.Reason;
            _userId = data.UserId;
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

            Platform.Log(LogLevel.Info, "Processing Series Level Deletion for Study {0}, A#: {1}",
                                         study.StudyInstanceUid, study.AccessionNumber);

            _seriesToDelete = new List<Series>();
            bool completed = false;
            try
            {
                WebDeleteSeriesLevelQueueData queueData = XmlUtils.Deserialize<WebDeleteSeriesLevelQueueData>(item.Data);

                using (ServerCommandProcessor processor = new ServerCommandProcessor(String.Format("Deleting Series from study {0}, A#:{1}, Patient: {2}, ID:{3}",
                            study.StudyInstanceUid, study.AccessionNumber, study.PatientsName, study.PatientId)))
                {
                    StudyXml studyXml = StorageLocation.LoadStudyXml();
                    IList<Series> existingSeries = StorageLocation.Study.Series;

                    // Add commands to delete the folders and update the xml
                    foreach (string seriesUid in queueData.SeriesInstanceUids)
                    {
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
                    }

                    // Add commands to update the db.. these commands are executed at the end.
                    foreach (string seriesUid in queueData.SeriesInstanceUids)
                    {
                        // Delete from DB
                        Series theSeries = CollectionUtils.SelectFirst(existingSeries, delegate(Series series) { return series.SeriesInstanceUid == seriesUid; });
                        if (theSeries!=null)
                        {
                            _seriesToDelete.Add(theSeries);
                            DeleteSeriesFromDBCommand delSeries = new DeleteSeriesFromDBCommand(StorageLocation, theSeries);
                            processor.AddCommand(delSeries);
                            delSeries.Executing += new EventHandler(DeleteSeriesFromDB_Executing);
                        }
                    }

                    processor.AddCommand(new SaveXmlCommand(studyXml, StorageLocation));

                    if (!processor.Execute())
                        throw new ApplicationException(
                            String.Format("Error occurred when series from Study {0}, A#: {1}",
                                         study.StudyInstanceUid, study.AccessionNumber));
                    else
                    {
                        foreach (Series series in _seriesToDelete)
                        {
                            OnSeriesDeleted(series);
                        }
                    }
                }


                completed = true;
            }
            finally
            {
                if (completed)
                {
                    OnCompleted();
                    PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                }
                else
                {
                    PostProcessing(item, WorkQueueProcessorStatus.Pending, WorkQueueProcessorDatabaseUpdate.None);
                }
            }
            
        }

        private void OnCompleted()
        {
            EnsureWebDeleteExtensionsLoaded();
            foreach (IWebDeleteProcessorExtension extension in _extensions)
            {
                try
                {
                    WebDeleteProcessorContext context = new WebDeleteProcessorContext(this, Level, StorageLocation, Reason, UserID);
                    extension.OnCompleted(context, _seriesToDelete);
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Error occurred in the extension but was ignored");
                }
            }
        }


        private void DeleteSeriesFromDB_Executing(object sender, EventArgs e)
        {
            DeleteSeriesFromDBCommand cmd = sender as DeleteSeriesFromDBCommand;
            OnDeletingSeriesInDatabase(cmd.Series);
        }

        private void OnDeletingSeriesInDatabase(Series series)
        {
            EnsureWebDeleteExtensionsLoaded();
            foreach(IWebDeleteProcessorExtension extension in _extensions)
            {
                try
                {
                    WebDeleteProcessorContext context = new WebDeleteProcessorContext(this, Level, StorageLocation, Reason, UserID);
                    extension.OnSeriesDeleting(context, series);
                }   
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Error occurred in the extension but was ignored");
                }
            }

        }
        private void OnSeriesDeleted(Series seriesUid)
        {
            EnsureWebDeleteExtensionsLoaded();
            foreach (IWebDeleteProcessorExtension extension in _extensions)
            {
                try
                {
                    WebDeleteProcessorContext context = new WebDeleteProcessorContext(this, Level, StorageLocation, Reason, UserID);
                    extension.OnSeriesDeleted(context, seriesUid);
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Error occurred in the extension but was ignored");
                }
            }
        }

        private IList<IWebDeleteProcessorExtension> EnsureWebDeleteExtensionsLoaded()
        {
            if (_extensions==null)
            {
                WebDeleteProcessorExtensionPoint xp = new WebDeleteProcessorExtensionPoint();
                _extensions = CollectionUtils.Cast<IWebDeleteProcessorExtension>(xp.CreateExtensions());    
            }

            return _extensions;
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

        
        #endregion
    
    }

    internal class DeleteSeriesCommandProcessorEventArgs:EventArgs
    {
        private readonly string _seriesInstanceUid;

        public DeleteSeriesCommandProcessorEventArgs(string seriesInstanceUid)
        {
            _seriesInstanceUid = seriesInstanceUid;
        }

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
        }
    }
}
