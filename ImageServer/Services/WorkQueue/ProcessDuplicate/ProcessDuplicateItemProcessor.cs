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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;


namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    class ProcessDuplicateItemProcessor : BaseItemProcessor
    {
        #region Private Member
		
        private WorkQueueProcessDuplicateSop _processDuplicateEntry;
        private List<BaseImageLevelUpdateCommand> _studyUpdateCommands;
        private List<BaseImageLevelUpdateCommand> _duplicateUpdateCommands;
        private StudyInformation _currentStudyInfo;
        private PatientNameRules _patientNameRules;

        #endregion

        #region Protected Properties

        protected String DuplicateFolder
        {
            get
            {
                if (_processDuplicateEntry == null)
                {
                    _processDuplicateEntry = new WorkQueueProcessDuplicateSop(WorkQueueItem);

                }

                return _processDuplicateEntry.GetDuplicateSopFolder();
            }
        }

        protected bool HistoryLogged { get; set; }

        #endregion

        #region Overridden Protected Methods

        protected override bool CanStart()
        {
            // If the study is not in processing state, attempt to push it into this state
            // If it fails, postpone the processing instead of failing
            if (StorageLocation.QueueStudyStateEnum != QueueStudyStateEnum.ProcessingScheduled)
            {
            	string failureReason;
                if (!ServerHelper.LockStudy(WorkQueueItem.StudyStorageKey, QueueStudyStateEnum.ProcessingScheduled, out failureReason))
                {
                    Platform.Log(LogLevel.Debug,
                                 "ProcessDuplicate cannot start at this point. Study is being locked by another processor. WriteLock Failure reason={0}",
                                 failureReason);
                    PostponeItem(String.Format("Study is being locked by another processor: {0}", failureReason));
                    return false;
                }
            }

            return true; // it is being locked by me
        }

        protected override void Initialize(Model.WorkQueue item)
        {
            base.Initialize(item);

            _processDuplicateEntry = new WorkQueueProcessDuplicateSop(item);
            _patientNameRules = new PatientNameRules(Study);
            HistoryLogged = _processDuplicateEntry.QueueData != null && _processDuplicateEntry.QueueData.State.HistoryLogged;
        }

        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckMemberIsSet(StorageLocation, "StorageLocation");
            Platform.CheckForNullReference(Study, "Study doesn't exist");
            
            if (WorkQueueUidList.Count == 0)
            {
                // we are done. Just need to cleanup the duplicate folder
                Platform.Log(LogLevel.Info, "{0} is completed. Cleaning up duplicate storage folder. (GUID={1}, action={2})",
                             item.WorkQueueTypeEnum, item.GetKey().Key, _processDuplicateEntry.QueueData.Action);
                
                DirectoryInfo duplicateFolder = new DirectoryInfo(DuplicateFolder);
                if (duplicateFolder.Exists)
                {
                    DirectoryUtility.DeleteIfEmpty(duplicateFolder.FullName);
                    DirectoryUtility.DeleteIfEmpty(duplicateFolder.Parent.FullName);
                }
                PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
            }
            else
            {
                Platform.Log(LogLevel.Info, "Processing {0} entry (GUID={1}, action={2})",
                             item.WorkQueueTypeEnum, item.GetKey().Key, _processDuplicateEntry.QueueData.Action);

                Platform.CheckTrue(Directory.Exists(DuplicateFolder), String.Format("Duplicate Folder {0} doesn't exist.", DuplicateFolder));

                LogWorkQueueInfo();

                EnsureStorageLocationIsWritable(StorageLocation);

                _currentStudyInfo = StudyInformation.CreateFrom(Study);

                ImageSetDetails duplicateSopDetails = null;

                // If deleting duplicates then don't log the history
                if (_processDuplicateEntry.QueueData.Action != ProcessDuplicateAction.Delete && !HistoryLogged)
                {
                    duplicateSopDetails = LoadDuplicateDetails();
                }

                try
                {

                    UpdateStudyOrDuplicates();

                    int count = ProcessUidList();

                    // If deleting duplicates then don't log the history
                    if (_processDuplicateEntry.QueueData.Action != ProcessDuplicateAction.Delete &&
                        !HistoryLogged && duplicateSopDetails != null && count > 0)
                    {
                        LogHistory(duplicateSopDetails);
                    }

                    PostProcessing(item, WorkQueueProcessorStatus.Pending, WorkQueueProcessorDatabaseUpdate.None);
                }
                finally
                {
                    UpdateQueueData();
                }
            }

        }

        private void EnsureStorageLocationIsWritable(StudyStorageLocation location)
        {
            FilesystemMonitor.Instance.EnsureStorageLocationIsWritable(location);
        }

        #endregion

        #region Private Methods

        private void UpdateStudyOrDuplicates()
        {
            switch (_processDuplicateEntry.QueueData.Action)
            {
                case ProcessDuplicateAction.OverwriteUseDuplicates:

                    if (_processDuplicateEntry.QueueData.State.ExistingStudyUpdated)
                        Platform.Log(LogLevel.Info, "Existing Study has been updated before");
                    else
                    {
                        Platform.Log(LogLevel.Info, "Update Existing Study w/ Duplicate Info");
                        _studyUpdateCommands = BuildUpdateStudyCommandsFromDuplicate();
                        using (ServerCommandProcessor processor = new ServerCommandProcessor("Update Existing Study w/ Duplicate Info"))
                        {
                            processor.AddCommand(new UpdateStudyCommand(ServerPartition, StorageLocation, _studyUpdateCommands, ServerRuleApplyTimeEnum.SopProcessed));
                            if (!processor.Execute())
                            {
                                throw new ApplicationException(processor.FailureReason, processor.FailureException);
                            }

                            _processDuplicateEntry.QueueData.State.ExistingStudyUpdated = true;
                        }
                    }
                    
                    break;
                    
                case ProcessDuplicateAction.OverwriteUseExisting:
                    ImageUpdateCommandBuilder commandBuilder = new ImageUpdateCommandBuilder();
                    _duplicateUpdateCommands = new List<BaseImageLevelUpdateCommand>();
                    _duplicateUpdateCommands.AddRange(commandBuilder.BuildCommands<StudyMatchingMap>(StorageLocation));
                    PrintCommands(_duplicateUpdateCommands);
                    break;
            }
        }

        private int ProcessUidList()
        {
            int count = 0;

            Platform.Log(LogLevel.Info, "Processing {0} duplicates...", WorkQueueUidList.Count);
            foreach (WorkQueueUid uid in WorkQueueUidList)
            {
                ProcessUid(uid);
                count++;
            }

            return count;
        }

        private void UpdateQueueData()
        {
            using(IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                // make a copy of the current queue data with updated info
                ProcessDuplicateQueueEntryQueueData data = new ProcessDuplicateQueueEntryQueueData
                                                               {
                                                                   Action = _processDuplicateEntry.QueueData.Action,
                                                                   DuplicateSopFolder = _processDuplicateEntry.QueueData.DuplicateSopFolder,
                                                                   State = new ProcessDuplicateQueueState
                                                                               {
                                                                                   HistoryLogged = HistoryLogged,
                                                                                   ExistingStudyUpdated = _processDuplicateEntry.QueueData.State.ExistingStudyUpdated
                                                                               }
                                                               };
                
                // update the queue data in db
                IWorkQueueEntityBroker broker = ctx.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueUpdateColumns parameters = new WorkQueueUpdateColumns
                                                        {
                                                            Data = XmlUtils.SerializeAsXmlDoc(data)
                                                        };
                if (broker.Update(WorkQueueItem.Key, parameters))
                {
                    ctx.Commit();
                    HistoryLogged = _processDuplicateEntry.QueueData.State.HistoryLogged = true;
                }
                
            }
        }

        private ImageSetDetails LoadDuplicateDetails()
        {
            IList<WorkQueueUid> uids = LoadAllWorkQueueUids();
            ImageSetDetails details = new ImageSetDetails();
            foreach(WorkQueueUid uid in  uids)
            {
                DicomFile file = LoadDuplicateDicomFile(uid, true);
                details.InsertFile(file);
            }

            return details;
        }

        private IList<WorkQueueUid> LoadAllWorkQueueUids()
        {
            using(IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IWorkQueueUidEntityBroker broker = context.GetBroker<IWorkQueueUidEntityBroker>();
                WorkQueueUidSelectCriteria criteria = new WorkQueueUidSelectCriteria();
                criteria.WorkQueueKey.EqualTo(WorkQueueItem.Key);
                return broker.Find(criteria);
            }
        }

        private void LogWorkQueueInfo()
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine(String.Format("\tGUID={0}", _processDuplicateEntry.GetKey().Key));
            log.AppendLine(String.Format("\tType={0}", _processDuplicateEntry.WorkQueueTypeEnum));
            log.AppendLine(String.Format("\tDuplicate Folder={0}", _processDuplicateEntry.GetDuplicateSopFolder()));
            log.AppendLine(String.Format("\tDuplicate Counts (this run)={0}", WorkQueueUidList.Count));
            log.AppendLine(String.Format("\tAction ={0}", _processDuplicateEntry.QueueData.Action));

            Platform.Log(LogLevel.Info, log);
        }

        private void LogHistory(ImageSetDetails details)
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                Platform.Log(LogLevel.Info, "Logging study history record...");
                IStudyHistoryEntityBroker broker = ctx.GetBroker<IStudyHistoryEntityBroker>();
                StudyHistoryUpdateColumns recordColumns = CreateStudyHistoryRecord(details);
                StudyHistory entry = broker.Insert(recordColumns);
                if (entry != null)
                    ctx.Commit();
                else
                    throw new ApplicationException("Unable to log study history record");
            }

            HistoryLogged = true;
        }

        private StudyHistoryUpdateColumns CreateStudyHistoryRecord(ImageSetDetails details)
        {
            StudyHistoryUpdateColumns columns = new StudyHistoryUpdateColumns
                                                	{
                                                		InsertTime = Platform.Time,
                                                		StudyHistoryTypeEnum = StudyHistoryTypeEnum.Duplicate,
                                                		StudyStorageKey = StorageLocation.GetKey(),
                                                		DestStudyStorageKey = StorageLocation.GetKey(),
                                                		StudyData = XmlUtils.SerializeAsXmlDoc(_currentStudyInfo)
                                                	};

        	ProcessDuplicateChangeLog changeLog = new ProcessDuplicateChangeLog
                                                  	{
                                                  		Action = _processDuplicateEntry.QueueData.Action,
                                                        DuplicateDetails = details,
                                                  		StudySnapShot = _currentStudyInfo,
                                                  		StudyUpdateCommands = _studyUpdateCommands
                                                  	};
        	XmlDocument doc = XmlUtils.SerializeAsXmlDoc(changeLog);
            columns.ChangeDescription = doc;
            return columns;
        }

        private void ProcessUid(WorkQueueUid uid)
        {
            switch(_processDuplicateEntry.QueueData.Action)
            {
                case ProcessDuplicateAction.Delete:
                    DeleteDuplicate(uid);
                    break;

                case ProcessDuplicateAction.OverwriteUseDuplicates:
                    OverwriteExistingInstance(uid, ProcessDuplicateAction.OverwriteUseDuplicates);
                    break;

                case ProcessDuplicateAction.OverwriteUseExisting:
                    OverwriteExistingInstance(uid, ProcessDuplicateAction.OverwriteUseExisting);
                    break;

                case ProcessDuplicateAction.OverwriteAsIs:
                    OverwriteExistingInstance(uid, ProcessDuplicateAction.OverwriteAsIs);
                    break;

                default:
                    throw new NotSupportedException(
                        String.Format("Not supported action: {0}", _processDuplicateEntry.QueueData.Action));
            }
        }

        private void DeleteDuplicate(WorkQueueUid uid)
        {
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Delete Received Duplicate"))
            {
                FileInfo duplicateFile = GetDuplicateSopFile(uid);
                processor.AddCommand(new DeleteFileCommand(duplicateFile.FullName));
                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));
                if (!processor.Execute())
                {
                    throw new ApplicationException(processor.FailureReason, processor.FailureException);
                }
            	Platform.Log(ServerPlatform.InstanceLogLevel, "Discard duplicate SOP {0} in {1}", uid.SopInstanceUid, duplicateFile.FullName);
            }
        }


        private bool ExistsInStudy(WorkQueueUid uid)
        {
            String path = StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);
            if (File.Exists(path))
                return true;

            // check the study xml
            StudyXml studyXml = StorageLocation.LoadStudyXml();
            return studyXml[uid.SeriesInstanceUid] != null &&
                   studyXml[uid.SeriesInstanceUid][uid.SopInstanceUid] != null;
        }

        private void OverwriteExistingInstance(WorkQueueUid uid, ProcessDuplicateAction action)
        {
            if (ExistsInStudy(uid))
            {
                // remove the existing image and update the count
                RemoveExistingImage(uid);
            }

            DicomFile duplicateDicomFile = LoadDuplicateDicomFile(uid, false);
            PreprocessDuplicate(duplicateDicomFile, action);
            AddDuplicateToStudy(duplicateDicomFile, uid, action);
        }

        private void PreprocessDuplicate(DicomFile duplicateDicomFile, ProcessDuplicateAction action)
        {
            _patientNameRules.Apply(duplicateDicomFile);

            if (action==ProcessDuplicateAction.OverwriteUseExisting)
            {
                foreach (BaseImageLevelUpdateCommand command in _duplicateUpdateCommands)
                {
                    if (!command.Apply(duplicateDicomFile))
                        throw new ApplicationException(String.Format("Unable to update the duplicate sop. Command={0}", command));
                }
            }
        }

        private void AddDuplicateToStudy(DicomFile duplicateDicomFile, WorkQueueUid uid, ProcessDuplicateAction action)
        {
            
            StudyProcessorContext context = new StudyProcessorContext(StorageLocation);
            SopInstanceProcessor sopInstanceProcessor = new SopInstanceProcessor(context) { EnforceNameRules = true };
            string group = uid.GroupID ?? ServerHelper.GetUidGroup(duplicateDicomFile, ServerPartition, WorkQueueItem.InsertTime);

            StudyXml studyXml = StorageLocation.LoadStudyXml();
            int originalInstanceCount = studyXml.NumberOfStudyRelatedInstances;

            bool compare = action != ProcessDuplicateAction.OverwriteAsIs;
            // NOTE: "compare" has no effect for OverwriteUseExisting or OverwriteUseDuplicate
            // because in both cases, the study and the duplicates are modified to be the same.
            ProcessingResult result = sopInstanceProcessor.ProcessFile(group, duplicateDicomFile, studyXml, compare, uid, duplicateDicomFile.Filename);
            if (result.Status == ProcessingStatus.Failed)
            {
                throw new ApplicationException("Unable to process file");
            }

            Debug.Assert(studyXml.NumberOfStudyRelatedInstances == originalInstanceCount + 1);
            Debug.Assert(File.Exists(StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid)));

        }

        private void RemoveExistingImage(WorkQueueUid uid)
        {
            string path = StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);

            if (!File.Exists(path))
                return;

            StudyXml studyXml = StorageLocation.LoadStudyXml();
            DicomFile file = new DicomFile(path);
            file.Load(DicomReadOptions.DoNotStorePixelDataInDataSet); // don't need to load pixel data cause we will delete it

            #if DEBUG
            int originalInstanceCountInXml = studyXml.NumberOfStudyRelatedInstances;
            int originalStudyInstanceCount = Study.NumberOfStudyRelatedInstances;
            int originalSeriesInstanceCount = Study.Series[uid.SeriesInstanceUid].NumberOfSeriesRelatedInstances;
            #endif

            using (ServerCommandProcessor processor = new ServerCommandProcessor("Delete Existing Image"))
            {
                processor.AddCommand(new DeleteFileCommand(path));
                processor.AddCommand(new RemoveInstanceFromStudyXmlCommand(StorageLocation, studyXml, file));
                processor.AddCommand(new UpdateInstanceCountCommand(StorageLocation, file));

                if (!processor.Execute())
                {
                    throw new ApplicationException(String.Format("Unable to remove existing image {0}", file.Filename), processor.FailureException);
                }
            }

            #if DEBUG
            Debug.Assert(!File.Exists(path));
            Debug.Assert(studyXml.NumberOfStudyRelatedInstances == originalInstanceCountInXml - 1);
            Debug.Assert(Study.Load(Study.Key).NumberOfStudyRelatedInstances == originalStudyInstanceCount - 1);
            Debug.Assert(Study.Load(Study.Key).Series[uid.SeriesInstanceUid].NumberOfSeriesRelatedInstances == originalSeriesInstanceCount - 1);
            #endif
        }

        private DicomFile LoadDuplicateDicomFile(WorkQueueUid uid, bool skipPixelData)
        {
            FileInfo duplicateFile = GetDuplicateSopFile(uid);
            Platform.CheckTrue(duplicateFile.Exists, String.Format("Duplicate SOP doesn't exist at {0}", uid.SopInstanceUid));
            DicomFile file = new DicomFile(duplicateFile.FullName);

            file.Load(skipPixelData? DicomReadOptions.StorePixelDataReferences:DicomReadOptions.Default);
            return file;
        }

        /// <summary>
        /// Gets the commands to update the study 
        /// </summary>
        /// <returns></returns>
        private List<BaseImageLevelUpdateCommand> BuildUpdateStudyCommandsFromDuplicate()
        {
            List<BaseImageLevelUpdateCommand> commands = new List<BaseImageLevelUpdateCommand>();
            if (WorkQueueUidList.Count>0)
            {
                WorkQueueUid uid = WorkQueueUidList[0];
                DicomFile file = LoadDuplicateDicomFile(uid, true);
                ImageUpdateCommandBuilder commandBuilder = new ImageUpdateCommandBuilder();
                // Create a list of commands to update the existing study based on what's defined in StudyMatchingMap
                // The value will be taken from the content of the duplicate image.
                commands.AddRange(commandBuilder.BuildCommands<StudyMatchingMap>(file.DataSet, 
                        new[]{  
                                new ServerEntityAttributeProvider(StorageLocation.Study),
                                new ServerEntityAttributeProvider(StorageLocation.Patient)
                             }));
            }

            return commands;
        }

        private FileInfo GetDuplicateSopFile(WorkQueueUid uid)
        {
            string baseDir = Path.Combine(StorageLocation.FilesystemPath, StorageLocation.PartitionFolder);
            baseDir = Path.Combine(baseDir, "Reconcile");
            baseDir = Path.Combine(baseDir, WorkQueueItem.GroupID);

            String path = Path.Combine(baseDir, uid.RelativePath);

            return new FileInfo(path);

        }

        private void PrintCommands(ICollection<BaseImageLevelUpdateCommand> commands)
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine("Update on duplicate sops:");
            if (commands != null && commands.Count > 0)
            {
                foreach (BaseImageLevelUpdateCommand command in _duplicateUpdateCommands)
                {
                    log.AppendLine(String.Format("\t{0}", command));
                }
            }

            Platform.Log(LogLevel.Info, log);

        }

        #endregion
    }
}
