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
using System.Threading;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    class ProcessDuplicateItemProcessor : BaseItemProcessor
    {
        private WorkQueueProcessDuplicateSop _processDuplicateEntry;
        private List<BaseImageLevelUpdateCommand> _studyUpdateCommands;
        private List<BaseImageLevelUpdateCommand> _duplicateUpdateCommands;
        private StudyInformation _originalStudyInfo;
        private ImageSetDetails _duplicateSopDetails;

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

        protected override bool CanStart()
        {
            // If the study is not in processing state, attempt to push it into this state
            // If it fails, postpone the processing instead of failing
            if (StorageLocation.QueueStudyStateEnum != QueueStudyStateEnum.ProcessingScheduled)
            {
                if (!LockStudyState(WorkQueueItem, QueueStudyStateEnum.ProcessingScheduled))
                {
                    Platform.Log(LogLevel.Debug,
                                 "ProcessDuplicate cannot start at this point. Study is being locked by another processor. Current state={0}",
                                 StorageLocation.QueueStudyStateEnum);
                    PostponeItem(WorkQueueItem);
                    return false;
                }
            }

            return true; // it is being locked by me
        }

        protected override void OnProcessItemBegin(Model.WorkQueue item)
        {
            base.OnProcessItemBegin(item);
            
            Platform.CheckForNullReference(Study, "Study doesn't exist");

        }

        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckMemberIsSet(StorageLocation, "StorageLocation");
            _processDuplicateEntry = new WorkQueueProcessDuplicateSop(item);

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

                _originalStudyInfo = StudyInformation.CreateFrom(Study);
           
                switch (_processDuplicateEntry.QueueData.Action)
                {
                    case ProcessDuplicateAction.OverwriteUseDuplicates:
                        Platform.Log(LogLevel.Info, "Update Existing Study w/ Duplicate Info");
                        _studyUpdateCommands = GetStudyUpdateCommands();
                        using (ServerCommandProcessor processor = new ServerCommandProcessor("Update Existing Study w/ Duplicate Info"))
                        {
                            processor.AddCommand(new UpdateStudyCommand(ServerPartition, StorageLocation, _studyUpdateCommands));
                            if (!processor.Execute())
                            {
                                throw new ApplicationException(processor.FailureReason, processor.FailureException);
                            }
                        }
                        break;
                    
                    case ProcessDuplicateAction.OverwriteUseExisting:
                        ImageUpdateCommandBuilder commandBuilder = new ImageUpdateCommandBuilder();
                        _duplicateUpdateCommands = new List<BaseImageLevelUpdateCommand>();
                        _duplicateUpdateCommands.AddRange(commandBuilder.BuildCommands<Study>(StorageLocation));
                        PrintCommands(_duplicateUpdateCommands);
                        break;
                }

                Platform.Log(LogLevel.Info, "Processing {0} duplicates...", WorkQueueUidList.Count);
                foreach (WorkQueueUid uid in WorkQueueUidList)
                {
                    ProcessUid(uid);
                }

                LogHistory();
                PostProcessing(item, WorkQueueProcessorStatus.Pending, WorkQueueProcessorDatabaseUpdate.None);
                
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

        private void LogHistory()
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                Platform.Log(LogLevel.Info, "Logging study history record...");
                IStudyHistoryEntityBroker broker = ctx.GetBroker<IStudyHistoryEntityBroker>();
                StudyHistoryUpdateColumns recordColumns = CreateStudyHistoryRecord();
                StudyHistory entry = broker.Insert(recordColumns);
                if (entry != null)
                    ctx.Commit();
                else
                    throw new ApplicationException("Unable to log study history record");
            }
        }

        private StudyHistoryUpdateColumns CreateStudyHistoryRecord()
        {
            StudyHistoryUpdateColumns columns = new StudyHistoryUpdateColumns();
            columns.InsertTime = Platform.Time;
            columns.StudyHistoryTypeEnum = StudyHistoryTypeEnum.Duplicate;
            columns.StudyStorageKey = StorageLocation.GetKey();
            columns.DestStudyStorageKey = StorageLocation.GetKey();

            columns.StudyData = XmlUtils.SerializeAsXmlDoc(_originalStudyInfo);

            ProcessDuplicateChangeLog changeLog = new ProcessDuplicateChangeLog();
            changeLog.Action = _processDuplicateEntry.QueueData.Action;
            changeLog.DuplicateDetails = _duplicateSopDetails;
            changeLog.StudySnapShot = _originalStudyInfo;
            changeLog.StudyUpdateCommands = _studyUpdateCommands;
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
                try
                {
                    DicomFile dcmFile = new DicomFile(duplicateFile.FullName);
                    dcmFile.Load(DicomReadOptions.DoNotStorePixelDataInDataSet);

                    if (_duplicateSopDetails == null)
                        _duplicateSopDetails = new ImageSetDetails(dcmFile.DataSet);

                    _duplicateSopDetails.InsertFile(dcmFile);

                }
                catch(DicomException ex)
                {
                    // error reading the duplicate... do we care if we are deleting it anyway?
                    Platform.Log(LogLevel.Warn, "Unable to read duplicate file: {0}. It will be deleted anyway. File={1}", ex.Message, duplicateFile.FullName);
                }
                

                processor.AddCommand(new DeleteFileCommand(duplicateFile.FullName));
                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));
                if (!processor.Execute())
                {
                    throw new ApplicationException(processor.FailureReason, processor.FailureException);
                }
                else
                {
                    Platform.Log(ServerPlatform.InstanceLogLevel, "Discard duplicate SOP {0} in {1}", uid.SopInstanceUid, duplicateFile.FullName);
                }
            }
        }

        private void OverwriteExistingInstance(WorkQueueUid uid, ProcessDuplicateAction action)
        {
            StudyXml studyXml = StorageLocation.LoadStudyXml();
            String finalDestination = StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);
            bool needOverwrite = File.Exists(finalDestination); // the file may not be there any more for some reason
            if (needOverwrite)
            {
                // The the instance is in the folder, it must have been processed. Otherwise, we can't proceed.
                Debug.Assert(studyXml.FindInstanceXml(uid.SeriesInstanceUid, uid.SopInstanceUid) != null, "Existing sop hasn't been processed");
            }

            using (ServerCommandProcessor processor = new ServerCommandProcessor("Move Duplicate Into Study Folder"))
            {
                DicomFile file = LoadDicomFile(uid);
                String dupFilePath = file.Filename;

                if (_duplicateSopDetails==null)
                    _duplicateSopDetails = new ImageSetDetails(file.DataSet);
                
                _duplicateSopDetails.InsertFile(file);

                if (needOverwrite)
                {
                    processor.AddCommand(new DeleteFileCommand(StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid)));
                    processor.AddCommand(new RemoveInstanceFromStudyXmlCommand(StorageLocation, studyXml, file));
                    processor.AddCommand(new UpdateInstanceCountCommand(StorageLocation, file));
                }

                switch(action)
                {
                    case ProcessDuplicateAction.OverwriteUseDuplicates:
                        // update the study using the duplicate demographics
                        // assume this has been done at the beginning
                        break;

                    case ProcessDuplicateAction.OverwriteUseExisting:
                        // update the duplicate demographics using the existing study
                        
                        processor.AddCommand(new DuplicateSopUpdateCommand(file, _duplicateUpdateCommands));
                
                        break;
                }

                
                bool compare = action != ProcessDuplicateAction.OverwriteAsIs;
                // NOTE: "compare" has no effect for OverwriteUseExisting or OverwriteUseDuplicate
                // because in both cases, the study and the duplicates are modified to be the same.

                ProcessReplacedSOPInstance processReplaced =
                        new ProcessReplacedSOPInstance(StorageLocation.ServerPartition, studyXml, file, compare);
                processor.AddCommand(processReplaced);

                processor.AddCommand(new DeleteFileCommand(file.Filename));
                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

                if (!processor.Execute())
                {
                    throw new ApplicationException(processor.FailureReason, processor.FailureException);
                }
                else
                {
                    if (needOverwrite)
						Platform.Log(ServerPlatform.InstanceLogLevel, "Replaced existing SOP {0} with duplicate {1}", uid.SopInstanceUid, dupFilePath);
                    else
						Platform.Log(ServerPlatform.InstanceLogLevel, "Added duplicate SOP {0} from {1}", uid.SopInstanceUid, dupFilePath);
                }
            }
        }

        private DicomFile LoadDicomFile(WorkQueueUid uid)
        {
            FileInfo duplicateFile = GetDuplicateSopFile(uid);
            Platform.CheckTrue(duplicateFile.Exists, String.Format("Duplicate SOP doesn't exist at {0}", uid.SopInstanceUid));
            DicomFile file = new DicomFile(duplicateFile.FullName);
            file.Load();
            return file;
        }

        private List<BaseImageLevelUpdateCommand> GetStudyUpdateCommands()
        {
            List<BaseImageLevelUpdateCommand> commands = new List<BaseImageLevelUpdateCommand>();
            if (WorkQueueUidList.Count>0)
            {
                WorkQueueUid uid = WorkQueueUidList[0];

                DicomFile file = LoadDicomFile(uid);

                Study study = new Study();
                Patient patient = new Patient();
                file.DataSet.LoadDicomFields(study);
                file.DataSet.LoadDicomFields(patient);

                ImageUpdateCommandBuilder commandBuilder = new ImageUpdateCommandBuilder();
                commands.AddRange(commandBuilder.BuildCommands<Study>(file.DataSet));
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

        private void PrintCommands(IList<BaseImageLevelUpdateCommand> commands)
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

    }

    internal class DuplicateSopUpdateCommand : ServerCommand
    {
        private readonly List<BaseImageLevelUpdateCommand> _commands;
        private readonly DicomFile _file;

        public DuplicateSopUpdateCommand(DicomFile file, List<BaseImageLevelUpdateCommand> commands)
            :base("Duplicate SOP demographic update command", true)
        {
            _file = file;
            _commands = commands;
        }

        protected override void OnExecute()
        {
            if (_commands!=null)
            {
                foreach (BaseImageLevelUpdateCommand command in _commands)
                {
                    if (!command.Apply(_file))
                        throw new ApplicationException(
                            String.Format("Unable to update the duplicate sop. Command={0}", command));
                }
            }
            
        }

        
        protected override void OnUndo()
        {
            
        }
    }

    internal class UpdateInstanceCountCommand : ServerDatabaseCommand
    {
        private readonly StudyStorageLocation _studyLocation;
        private readonly DicomFile _file;

        public UpdateInstanceCountCommand(StudyStorageLocation studyLocation, DicomFile file)
            :base("Update Study Count", true)
        {
            _studyLocation = studyLocation;
            _file = file;
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            String seriesUid = _file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            String instanceUid = _file.DataSet[DicomTags.SopInstanceUid].ToString();
            
            IDeleteInstance deleteInstanceBroker = updateContext.GetBroker<IDeleteInstance>();
            DeleteInstanceParameters parameters = new DeleteInstanceParameters();
            parameters.StudyStorageKey = _studyLocation.GetKey();
            parameters.SeriesInstanceUid = seriesUid;
            parameters.SOPInstanceUid = instanceUid;
            if (!deleteInstanceBroker.Execute(parameters))
            {
                throw new ApplicationException("Unable to update instance count in db");
            }

        }
    }

    internal class RemoveInstanceFromStudyXmlCommand : ServerCommand
    {
        private readonly StudyStorageLocation _studyLocation;
        private readonly DicomFile _file;
        private readonly StudyXml _studyXml;

        public RemoveInstanceFromStudyXmlCommand(StudyStorageLocation location, StudyXml studyXml, DicomFile file)
            :base("Remove Instance From Study Xml", true)
        {
            _studyLocation = location;
            _file = file;
            _studyXml = studyXml;
        }

        protected override void OnExecute()
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
       
    }

    internal class ProcessReplacedSOPInstance : ServerDatabaseCommand
    {
        private readonly ServerPartition _partition;
        private readonly DicomFile _file;
        private StudyStorageLocation _storageLocation;
        private ProcessingResult _result;
        private readonly StudyXml _studyXml;
        private readonly bool _compare;

        public ProcessReplacedSOPInstance(ServerPartition partition, StudyXml studyXml, DicomFile file, bool compare)
            : base("ProcessReplacedSOPInstance", true)
        {
            _partition = partition;
            _file = file;
            _compare = compare;
            _studyXml = studyXml;
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            String studyUid = _file.DataSet[DicomTags.StudyInstanceUid].ToString();
            
            if (!FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(updateContext, _partition.GetKey(), studyUid, out _storageLocation))
            {
                throw new ApplicationException("No online storage found");
            }
            else
            {
                StudyProcessorContext context = new StudyProcessorContext(_storageLocation);
                SopInstanceProcessor sopInstanceProcessor = new SopInstanceProcessor(context);
                _result = sopInstanceProcessor.ProcessFile(_file, _studyXml, true, _compare);
                if (_result.Status == ProcessingStatus.Failed)
                {
                    throw new ApplicationException("Unable to process file");
                }
            }

        }

        protected override void OnUndo()
        {
            
        }
    }

    internal class OpValidatation : ServerCommand
    {
        private readonly Model.WorkQueue _item;
        private readonly WorkQueueUid _uid;
        private readonly IList<StudyStorageLocation> _locations;

        public OpValidatation(Model.WorkQueue item, WorkQueueUid uid)
            :base("Validation Command", true)
        {
            _item = item;
            _uid = uid;
            _locations = _item.LoadStudyLocations(Common.CommandProcessor.ExecutionContext.Current.ReadContext);
        }

        private String GetDuplicateSopFile()
        {
            _item.LoadStudy(ExecutionContext.ReadContext);

            string baseDir = Path.Combine(_locations[0].FilesystemPath, _locations[0].PartitionFolder);
            baseDir = Path.Combine(baseDir, "Reconcile");
            baseDir = Path.Combine(baseDir, _item.GroupID);

            String path = Path.Combine(baseDir, _uid.RelativePath);

            return path;

        }

        protected override void OnExecute()
        {
            // duplicate file is deleted
            String dupPath = GetDuplicateSopFile();
            String replacedSopPath = _locations[0].GetSopInstancePath(_uid.SeriesInstanceUid, _uid.SopInstanceUid);

            Platform.CheckTrue(!File.Exists(dupPath), "Duplicate sop was not deleted.");
            Platform.CheckTrue(File.Exists(replacedSopPath), "Replaced sop is not in the study folder.");

            #if DEBUG
            #region MORE EXTENSIVE CHECK
            // can load the file without problem
            DicomFile file = new DicomFile(replacedSopPath);
            file.Load();
            #endregion
            #endif
        }

        protected override void OnUndo()
        {
            // NO-OP
        }
    }
}
