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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy
{
    class ReconcileMergeStudyCommandParameters
    {
        public bool UpdateDestination;
        public List<BaseImageLevelUpdateCommand> Commands;
    }

    /// <summary>
    /// Command for reconciling images by merging new images into an existing study.
    /// </summary>
    /// <remark>
    /// </remark>
    class MergeStudyCommand : ReconcileCommandBase
    {
        #region Private Members
        private ServerCommandProcessor _processor;
        private readonly List<WorkQueueUid> _processedUidList = new List<WorkQueueUid>();
        private readonly List<WorkQueueUid> _failedUidList = new List<WorkQueueUid>();
        private readonly List<WorkQueueUid> _duplicateList = new List<WorkQueueUid>();
        private readonly ReconcileMergeStudyCommandParameters _parameters;

        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="MergeStudyCommand"/>
        /// </summary>
        public MergeStudyCommand(ReconcileStudyProcessorContext context, ReconcileMergeStudyCommandParameters parameters)
            : base("Merge Study", true, context)
        {
            _parameters = parameters;
        }
        #endregion

        #region Overriden Protected Methods
        protected override void OnExecute()
        {
            Platform.CheckForNullReference(Context, "Context");

            DetermineDestination();

            if (_parameters.UpdateDestination)
                UpdateExistingStudy();
            
            LoadMergedStudyEntities();

            ProcessUidList();
            
            UpdateHistory();

            LogResult();

        }

        private void DetermineDestination()
        {
            if (Context.DestStorageLocation != null)
                return;

            Context.DestStorageLocation = Context.WorkQueueItemStudyStorage; // merge into this study
        }

        #endregion

        #region Protected Methods
        protected override void OnUndo()
        {
            if (_processor != null)
            {
                _processor.Rollback();
                _processor = null;
            }
        }
        #endregion

        #region Private Members
        private void LogResult()
        {
            StringBuilder log = new StringBuilder();
            log.AppendFormat("Destination location: {0}", Context.DestStorageLocation.GetStudyPath());
            log.AppendLine();
            if (_failedUidList.Count > 0)
            {
                log.AppendFormat("{0} images failed to be reconciled.", _failedUidList.Count);
                log.AppendLine();
            }
            if (_duplicateList.Count > 0)
            {
                log.AppendFormat("{0} images are duplicate.", _duplicateList.Count);
                log.AppendLine();
            }
            
            log.AppendFormat("{0} images have been reconciled and will be processed.", _processedUidList.Count);
            log.AppendLine();
            Platform.Log(LogLevel.Info, log);
        }

        private void UpdateHistory()
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IStudyHistoryEntityBroker historyUpdateBroker = ctx.GetBroker<IStudyHistoryEntityBroker>();
                StudyHistoryUpdateColumns parms = new StudyHistoryUpdateColumns();
                parms.DestStudyStorageKey = Context.DestStorageLocation.GetKey();
                historyUpdateBroker.Update(Context.History.GetKey(), parms);


                ILockStudy lockStudyBroker = ctx.GetBroker<ILockStudy>();
                LockStudyParameters lockParms = new LockStudyParameters();
                lockParms.QueueStudyStateEnum = QueueStudyStateEnum.ProcessingScheduled;
                lockParms.StudyStorageKey = Context.WorkQueueItem.StudyHistoryKey;

                lockStudyBroker.Execute(lockParms);

                ctx.Commit();
            }
        }


        private void UpdateExistingStudy()
        {

            Platform.Log(LogLevel.Info, "Updating existing study...");
            using(ServerCommandProcessor updateProcessor = new ServerCommandProcessor("Update Study"))
            {
                UpdateStudyCommand studyUpdateCommand = new UpdateStudyCommand(Context.Partition, Context.DestStorageLocation, _parameters.Commands);
                updateProcessor.AddCommand(studyUpdateCommand);
                if (!updateProcessor.Execute())
                {
                    throw new ApplicationException(
                        String.Format("Unable to update existing study: {0}", updateProcessor.FailureReason));
                }
            }
            
        }

        private void ProcessUidList()
        {
            List<BaseImageLevelUpdateCommand> updateCommandList = BuildUpdateCommandList();
            PrintUpdateCommands(updateCommandList);
            int counter = 0;
            Platform.Log(LogLevel.Info, "Populating new images into study folder.. {0} to go", Context.WorkQueueUidList.Count);
            foreach (WorkQueueUid uid in Context.WorkQueueUidList)
            {
                using (ServerCommandProcessor processor = new ServerCommandProcessor("Reconciling image processor"))
                {
                    string imagePath = GetReconcileUidPath(uid);
                    DicomFile file = new DicomFile(imagePath);
                    file.Load();
                    foreach (BaseImageLevelUpdateCommand command in updateCommandList)
                    {
                        command.File = file;
                        processor.AddCommand(command);
                    }

                    processor.AddCommand(new SaveFileCommand(Context, file));
                    UpdateWorkQueueCommand.CommandParameters parameters = new UpdateWorkQueueCommand.CommandParameters();
                    parameters.SeriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                    parameters.SopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                    parameters.Extension = "dcm";
                    parameters.IsDuplicate = false;
                    processor.AddCommand(new UpdateWorkQueueCommand(Context, parameters));
                    processor.AddCommand(new FileDeleteCommand(GetReconcileUidPath(uid), true));
                    processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

                    //SaveFile(uid, file);
                    if (counter == 0)
                    {
                        processor.AddCommand(new UpdateHistoryCommand(Context));
                    }

                    if (!processor.Execute())
                    {
                        FailUid(uid, true);
                        throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", file.Filename, processor.FailureReason));
                    }

                    counter++;
                    _processedUidList.Add(uid);
                    Platform.Log(LogLevel.Info, "Reconciled SOP {0} (not yet processed) [{1} of {2}]", uid.SopInstanceUid, counter, Context.WorkQueueUidList.Count);
                }
            }

        }

        private void LoadMergedStudyEntities()
        {
            StudyStorage storage = StudyStorage.Load(Context.DestStorageLocation.GetKey());
            Context.DestStorageLocation = StudyStorageLocation.FindStorageLocations(storage)[0];
        }

        
        private void SaveFile(WorkQueueUid uid, DicomFile file)
        {
            String seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            String sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Update file system"))
            {
                String destPath = Context.DestStorageLocation.FilesystemPath;
                String extension = "dcm";

                processor.AddCommand(new CreateDirectoryCommand(destPath));

                destPath = Path.Combine(destPath, Context.DestStorageLocation.PartitionFolder);
                processor.AddCommand(new CreateDirectoryCommand(destPath));

                destPath = Path.Combine(destPath, Context.DestStorageLocation.StudyFolder);
                processor.AddCommand(new CreateDirectoryCommand(destPath));

                destPath = Path.Combine(destPath, Context.DestStorageLocation.StudyInstanceUid);
                processor.AddCommand(new CreateDirectoryCommand(destPath));

                destPath = Path.Combine(destPath, seriesInstanceUid);
                processor.AddCommand(new CreateDirectoryCommand(destPath));

                destPath = Path.Combine(destPath, sopInstanceUid);
                destPath += "."+ extension;

                bool duplicate = File.Exists(destPath);

                if (duplicate)
                {
                    if (uid != null)
                    {
                        Platform.Log(LogLevel.Warn, "Image {0} is a duplicate. Existing sop will be replaced.", file.Filename);
                        _duplicateList.Add(uid);
                    }
                }


                Platform.Log(LogLevel.Debug, "Saving {0}", destPath);
                processor.AddCommand(new SaveDicomFileCommand(destPath, file, false, true));
                processor.AddCommand(new FileDeleteCommand(GetReconcileUidPath(uid), true));

                UpdateWorkQueueCommand.CommandParameters parameters = new UpdateWorkQueueCommand.CommandParameters();
                parameters.Extension = extension;
                parameters.IsDuplicate = duplicate;
                parameters.SeriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                parameters.SopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                processor.AddCommand(new UpdateWorkQueueCommand(Context, parameters));
                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));
                
                if (!processor.Execute())
                {
                    _failedUidList.Add(uid);
                    throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", file.Filename, processor.FailureReason));
                }
                _processedUidList.Add(uid);
                
            }      
        }
        private List<BaseImageLevelUpdateCommand> BuildUpdateCommandList()
        {
            List<BaseImageLevelUpdateCommand> updateCommandList = new List<BaseImageLevelUpdateCommand>();
            
            ImageUpdateCommandBuilder builder = new ImageUpdateCommandBuilder();
            updateCommandList.AddRange(builder.BuildCommands<DemographicInfo>(Context.DestStorageLocation));
            updateCommandList.AddRange(builder.BuildCommands<StudyInfoMapping>(Context.DestStorageLocation));

            
            return updateCommandList;
        }

        #endregion

        #region Private Static Methods
        private static void PrintUpdateCommands(IEnumerable<BaseImageLevelUpdateCommand> updateCommandList)
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine("Update on merged images:");
            foreach (BaseImageLevelUpdateCommand cmd in updateCommandList)
            {
                log.AppendLine(String.Format("\t{0}", cmd));
            }
            Platform.Log(LogLevel.Info, log);
        }

        #endregion
    }

}
