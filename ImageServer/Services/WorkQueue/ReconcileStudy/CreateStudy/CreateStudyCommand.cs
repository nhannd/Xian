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
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy
{

    /// <summary>
    /// Command for reconciling images by creating a new study or merge into an existing study.
    /// </summary>
    class CreateStudyCommand : ReconcileCommandBase
    {
        internal class CommandParameters
        {
            public List<BaseImageLevelUpdateCommand> Commands;
        }

        private readonly CommandParameters _parameters;

        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="CreateStudyCommand"/>
        /// </summary>
        public CreateStudyCommand(ReconcileStudyProcessorContext context, CommandParameters parameters)
            : base("Create Study", true, context)
        {
            _parameters = parameters;
        }

        private CommandParameters Parameters
        {
            get { return _parameters; }
        }

        #endregion


        #region Overriden Protected Methods
        protected override void OnExecute()
        {
            Platform.CheckForNullReference(Context, "Context");
            Platform.CheckForNullReference(Context.WorkQueueItem, "Context.WorkQueueItem");
            Platform.CheckForNullReference(Context.WorkQueueUidList, "Context.WorkQueueUidList");

            PrintChangeList();
            ProcessUidList();
        }

        protected override void OnUndo()
        {

        }


        #endregion

        #region Private Methods

        private void PrintChangeList()
        {
            StringBuilder log = new StringBuilder();
            log.AppendFormat("Applying following changes to images:\n");
            foreach (BaseImageLevelUpdateCommand cmd in Parameters.Commands)
            {
                log.AppendFormat("{0}", cmd);
                log.AppendLine();
            }

            Platform.Log(LogLevel.Info, log);
        }

        private void ProcessUidList()
        {
            int counter = 0;
            Platform.Log(LogLevel.Info, "Populating images into study folder.. {0} to go", Context.WorkQueueUidList.Count);
            foreach (WorkQueueUid uid in Context.WorkQueueUidList)
            {
                using (ServerCommandProcessor processor = new ServerCommandProcessor("Reconciling image processor"))
                {
                    string path = GetReconcileUidPath(uid);
                    DicomFile file = new DicomFile(path);
                    file.Load(DicomReadOptions.StorePixelDataReferences);

                    foreach (BaseImageLevelUpdateCommand command in Parameters.Commands)
                    {
                        command.File = file;
                        processor.AddCommand(command);
                    }

                    if (Context.DestStorageLocation == null)
                    {
                        processor.AddCommand(new InitializeStorageCommand(Context, file));

                        processor.AddCommand(new CreateStudyFolderDirectory(Context, file));
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

                    if (counter == 0)
                    {
                        processor.AddCommand(new UpdateHistoryCommand(Context));
                    }

                    if (!processor.Execute())
                    {
                        FailUid(uid, true);
                        throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", file.Filename, processor.FailureReason));
                    }
                }

                counter++;
                Platform.Log(LogLevel.Info, "Reconciled SOP {0} (not yet processed) [{1} of {2}]", uid.SopInstanceUid, counter, Context.WorkQueueUidList.Count);

            }
        }

        #endregion
    }

    /// <summary>
    /// Command to initialize the study storage record in the database
    /// </summary>
    class InitializeStorageCommand : ServerDatabaseCommand<ReconcileStudyProcessorContext, DicomFile>
    {
        public InitializeStorageCommand(ReconcileStudyProcessorContext context, DicomFile file)
            :base("InitializeStorageCommand", true, context, file)
        {
            Platform.CheckForNullReference(file, "file");
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            DicomFile file = Parameters;

            if (Context.DestStorageLocation != null)
                return;
            else
            {
                Context.DestStorageLocation = FindOrCreateStudyStorageLocation(file);
            }
        }

        private StudyStorageLocation FindOrCreateStudyStorageLocation(DicomFile file)
        {
            Platform.CheckForNullReference(UpdateContext, "UpdateContext");
            Platform.CheckForNullReference(file, "file");
            Platform.CheckForNullReference(Context, "Context");
            Platform.CheckForNullReference(Context.Partition, "Context.Partition");
            
            
            String studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
            String studyDate = file.DataSet[DicomTags.StudyDate].ToString();

            String folder = StorageHelper.ResolveStorageFolder(Context.Partition, studyInstanceUid, studyDate, UpdateContext, true);
            
            IQueryStudyStorageLocation locQuery = UpdateContext.GetBroker<IQueryStudyStorageLocation>();
            StudyStorageLocationQueryParameters locParms = new StudyStorageLocationQueryParameters();
            locParms.StudyInstanceUid = studyInstanceUid;
            locParms.ServerPartitionKey = Context.Partition.GetKey();
            IList<StudyStorageLocation> studyLocationList = locQuery.Find(locParms);

            if (studyLocationList.Count == 0)
            {
                // INSERT NEW LOCATION INTO DB

                IStudyStorageEntityBroker selectBroker = UpdateContext.GetBroker<IStudyStorageEntityBroker>();
                StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();

                criteria.ServerPartitionKey.EqualTo(Context.Partition.GetKey());
                criteria.StudyInstanceUid.EqualTo(studyInstanceUid);

                StudyStorage storage = selectBroker.FindOne(criteria);
                if (storage != null)
                {
                    throw new Exception(String.Format("Received SOP Instances for Study in {0} state.  Rejecting image.", storage.StudyStatusEnum.Description));

                }

                FilesystemSelector selector = new FilesystemSelector(FilesystemMonitor.Instance);
                ServerFilesystemInfo filesystem = selector.SelectFilesystem(file);
                if (filesystem == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to select location for storing study.");

                    return null;
                }

                IInsertStudyStorage locInsert = UpdateContext.GetBroker<IInsertStudyStorage>();
                InsertStudyStorageParameters insertParms = new InsertStudyStorageParameters();
                insertParms.ServerPartitionKey = Context.Partition.GetKey();
                insertParms.StudyInstanceUid = studyInstanceUid;
                insertParms.Folder = folder;
                insertParms.FilesystemKey = filesystem.Filesystem.GetKey();
                insertParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;

                if (file.TransferSyntax.LosslessCompressed)
                {
                    insertParms.TransferSyntaxUid = file.TransferSyntax.UidString;
                    insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
                }
                else if (file.TransferSyntax.LossyCompressed)
                {
                    insertParms.TransferSyntaxUid = file.TransferSyntax.UidString;
                    insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossy;
                }
                else
                {
                    insertParms.TransferSyntaxUid = TransferSyntax.ExplicitVrLittleEndianUid;
                    insertParms.StudyStatusEnum = StudyStatusEnum.Online;
                }

                studyLocationList = locInsert.Find(insertParms);
            }
            else
            {
                if (!FilesystemMonitor.Instance.CheckFilesystemWriteable(studyLocationList[0].FilesystemKey))
                {
                    Platform.Log(LogLevel.Warn, "Unable to find writable filesystem for study {0} on Partition {1}",
                                 studyInstanceUid, Context.Partition.Description);
                    return null;
                }
            }

            //TODO:  Do we need to do something to identify a primary storage location?
            // Also, should the above check for writeable location check the other availab
            return studyLocationList[0];
        }
       
    }

    /// <summary>
    /// Command to create the study folder
    /// </summary>
    class CreateStudyFolderDirectory  : ServerCommand<ReconcileStudyProcessorContext, DicomFile>
    {
        private ServerCommandProcessor _processor;

        public CreateStudyFolderDirectory(ReconcileStudyProcessorContext context, DicomFile file)
            : base("SaveFileCommand", true, context, file)
        {
        }

        protected override void OnExecute()
        {
            Platform.CheckForNullReference(Context.DestStorageLocation, "Context.DestStorageLocation");

            String seriesInstanceUid = Parameters.DataSet[DicomTags.SeriesInstanceUid].ToString();

            String destPath = Context.DestStorageLocation.FilesystemPath;
            _processor = new ServerCommandProcessor("Create Directory Processor");

            _processor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, Context.DestStorageLocation.PartitionFolder);
            _processor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, Context.DestStorageLocation.StudyFolder);
            _processor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, Context.DestStorageLocation.StudyInstanceUid);
            _processor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, seriesInstanceUid);
            _processor.AddCommand(new CreateDirectoryCommand(destPath));

            if (!_processor.Execute())
            {
                throw new ApplicationException(
                    String.Format("Cannot create study folder: {0}", _processor.FailureReason));
            }

            Platform.Log(LogLevel.Info, "New study folder created: {0}", destPath);
        }

        protected override void OnUndo()
        {
            if (_processor!=null)
            {
                _processor.Rollback();
                _processor = null;
            }
        }
    }
}