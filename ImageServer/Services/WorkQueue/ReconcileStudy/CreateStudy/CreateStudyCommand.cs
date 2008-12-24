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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy
{
    /// <summary>
    /// Command for reconciling images by creating a new study or merge into an existing study.
    /// </summary>
    class CreateStudyCommand : ServerCommand, IDisposable
    {
        #region Private Members
        private ReconcileStudyProcessorContext _context;
        private ServerCommandProcessor _processor;
        private readonly List<WorkQueueUid> _processedUidList = new List<WorkQueueUid>();
        private readonly List<WorkQueueUid> _failedUidList = new List<WorkQueueUid>();
        private readonly List<WorkQueueUid> _duplicateList = new List<WorkQueueUid>(); 

        private readonly Dictionary<string, WorkQueueUid> _fileToUidMap = new Dictionary<string, WorkQueueUid>();

        private readonly string _workingDir = ServerPlatform.GetTempPath();

        private readonly List<BaseImageLevelUpdateCommand> _imageLevelCommands = new List<BaseImageLevelUpdateCommand>();
        private StudyStorageLocation _destStudyStorage = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="CreateStudyCommand"/>
        /// </summary>
        public CreateStudyCommand()
            : base("Create Study", true)
        {

        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a list of <see cref="BaseImageLevelUpdateCommand"/> used by the command to update the images.
        /// </summary>
        public List<BaseImageLevelUpdateCommand> ImageLevelCommands
        {
            get
            {
                return _imageLevelCommands;
            }
        }

        #endregion

        #region Overriden Protected Methods
        protected override void OnExecute()
        {
            Platform.CheckForNullReference(_context, "_context");

            PrintChangeList();
            PrepareWorkingFolder();
            UpdateFilesystems();
            UpdateHistory();
            LogResult();
   
            if (_processedUidList.Count != _context.WorkQueueUidList.Count)
            {
                if (_duplicateList.Count == _context.WorkQueueUidList.Count)
                {
                    throw new ApplicationException("Duplicate sops");
                }
                else
                {
                    throw new ApplicationException("Some of the sops can be processed.");
                }
            }
        }

        protected override void OnUndo()
        {
            if (_processor != null)
            {
                _processor.Rollback();
            }
        }


        #endregion

        #region Private Methods

        private void LogResult()
        {
            StringBuilder log = new StringBuilder();
            log.AppendFormat("Destination location: {0}", _destStudyStorage.GetStudyPath());
            log.AppendLine();
            if (_failedUidList.Count > 0)
            {
                log.AppendFormat("{0} images failed to be reconciled.", _failedUidList.Count);
                log.AppendLine();
            }
            if (_processedUidList.Count>0)
            {
                log.AppendFormat("{0} images have been reconciled and will be processed.", _processedUidList.Count);
                log.AppendLine();
            }
            if (_duplicateList.Count > 0)
            {
                log.AppendFormat("{0} images are duplicate.", _duplicateList.Count);
                log.AppendLine();
            }
            Platform.Log(LogLevel.Info, log);
        }

        private void PrintChangeList()
        {
            StringBuilder log = new StringBuilder();
            log.AppendFormat("Changes to be applied to images:\n");
            foreach (BaseImageLevelUpdateCommand cmd in _imageLevelCommands)
            {
                log.AppendFormat("{0}", cmd);
                log.AppendLine();
            }
            
            Platform.Log(LogLevel.Info, log);
        }


        private void UpdateHistory()
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using(IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IStudyHistoryEntityBroker historyUpdateBroker = ctx.GetBroker<IStudyHistoryEntityBroker>();
                StudyHistoryUpdateColumns parms = new StudyHistoryUpdateColumns();
                parms.DestStudyStorageKey =DestStudyStorage.GetKey();
                historyUpdateBroker.Update(_context.History.GetKey(), parms);

                ILockStudy lockStudyBroker = ctx.GetBroker<ILockStudy>();
                LockStudyParameters lockParms = new LockStudyParameters();
                lockParms.QueueStudyStateEnum = QueueStudyStateEnum.ProcessingScheduled;
                lockParms.StudyStorageKey = _context.WorkQueueItem.StudyHistoryKey;
                lockStudyBroker.Execute(lockParms);

                ctx.Commit();
            }
        }


        private void CleanupWorkingFolder()
        {
            DirectoryUtility.DeleteIfExists(_workingDir);
        }

        private void UpdateFilesystems()
        {
            
            FileProcessor.Process(_workingDir, "*.dcm",
                                  delegate(string path)
                                      {
                                          // this should be the updated study instance
                                          DicomFile file = new DicomFile(path);
                                          file.Load(DicomReadOptions.StorePixelDataReferences);
                      
                                          InitializeStorage(file);

                                          SaveFile(file, DestStudyStorage);

                                      }, true);
            
        }

        private StudyStorageLocation GetStudyStorageLocation(DicomFile file)
        {
            String studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            String studyDate = file.DataSet[DicomTags.StudyDate].GetString(0, ImageServerCommonConfiguration.DefaultStudyRootFolder);

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext updateContext = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IQueryStudyStorageLocation locQuery = updateContext.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters locParms = new StudyStorageLocationQueryParameters();
                locParms.StudyInstanceUid = studyInstanceUid;
                locParms.ServerPartitionKey = _context.Partition.GetKey();
                IList<StudyStorageLocation> studyLocationList = locQuery.Find(locParms);

                if (studyLocationList.Count == 0)
                {
                    // INSERT NEW LOCATION INTO DB

                    IStudyStorageEntityBroker selectBroker = updateContext.GetBroker<IStudyStorageEntityBroker>();
                    StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();

                    criteria.ServerPartitionKey.EqualTo(_context.Partition.GetKey());
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

                    IInsertStudyStorage locInsert = store.OpenReadContext().GetBroker<IInsertStudyStorage>();
                    InsertStudyStorageParameters insertParms = new InsertStudyStorageParameters();
                    insertParms.ServerPartitionKey = _context.Partition.GetKey();
                    insertParms.StudyInstanceUid = studyInstanceUid;
                    insertParms.Folder = studyDate;
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

                    updateContext.Commit();
                }
                else
                {
                    if (!FilesystemMonitor.Instance.CheckFilesystemWriteable(studyLocationList[0].FilesystemKey))
                    {
                        Platform.Log(LogLevel.Warn, "Unable to find writable filesystem for study {0} on Partition {1}",
                                     studyInstanceUid, _context.Partition.Description);
                        return null;
                    }
                }

                //TODO:  Do we need to do something to identify a primary storage location?
                // Also, should the above check for writeable location check the other availab
                return studyLocationList[0];
            }
        }
        
        private void InitializeStorage(DicomFile file)
        {
            if (_destStudyStorage != null)
                return;
            else
            {
                _destStudyStorage = GetStudyStorageLocation(file);
                
            }
        }



        private void SaveFile(DicomFile file, StudyStorageLocation destStudyStorage)
        {
            WorkQueueUid uid = _fileToUidMap[file.Filename];
            
            _processor = new ServerCommandProcessor("Save images");

            string workingImagePath = file.Filename;

            String seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            String sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                      
            String destPath = destStudyStorage.FilesystemPath;
            String extension = ".dcm";

            using (ServerCommandProcessor processor = new ServerCommandProcessor("Reconciling image processor"))
            {
                processor.AddCommand(new CreateDirectoryCommand(destPath));

                destPath = Path.Combine(destPath, destStudyStorage.PartitionFolder);
                processor.AddCommand(new CreateDirectoryCommand(destPath));

                destPath = Path.Combine(destPath, destStudyStorage.StudyFolder);
                processor.AddCommand(new CreateDirectoryCommand(destPath));

                destPath = Path.Combine(destPath, destStudyStorage.StudyInstanceUid);
                processor.AddCommand(new CreateDirectoryCommand(destPath));

                destPath = Path.Combine(destPath, seriesInstanceUid);
                processor.AddCommand(new CreateDirectoryCommand(destPath));

                destPath = Path.Combine(destPath, sopInstanceUid);
                destPath += extension;

                if (File.Exists(destPath))
                {
                    if (_fileToUidMap.ContainsKey(workingImagePath))
                    {
                        #region Duplicate SOP

                        // TODO: Add code to handle duplicate sop here
                        Platform.Log(LogLevel.Warn, "Image {0} cannot be processed because of duplicate in {1}", sopInstanceUid, destPath);
                        FailDuplicate(uid);
                        _failedUidList.Add(uid);
                        _duplicateList.Add(uid);
                        #endregion

                        return;
                    }
                }


                SaveDicomFileCommand saveCommand = new SaveDicomFileCommand(destPath, file);
                processor.AddCommand(saveCommand);
                processor.AddCommand(new UpdateWorkQueueCommand(file, destStudyStorage, extension, false));

                if (!processor.Execute())
                {
                    FailUid(uid, true);
                    throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", file.Filename, processor.FailureReason));
                }
            }
            

            if (_fileToUidMap.ContainsKey(workingImagePath))
            {
                DeleteUid(uid);
                File.Delete(GetUidPath(uid));
                _processedUidList.Add(uid);
            }

            
        }

        private static void FailDuplicate(WorkQueueUid sop)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueUidEntityBroker uidUpdateBroker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
                WorkQueueUidUpdateColumns columns = new WorkQueueUidUpdateColumns();
                columns.Failed = true;
                columns.Duplicate = true;
                columns.FailureCount = 1;
                
                uidUpdateBroker.Update(sop.GetKey(), columns);
                updateContext.Commit();
            }
        }

        private static void FailUid(WorkQueueUid sop, bool retry)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueUidEntityBroker uidUpdateBroker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
                WorkQueueUidUpdateColumns columns = new WorkQueueUidUpdateColumns();
                if (!retry)
                    columns.Failed = true;
                else
                {
                    if (sop.FailureCount >= WorkQueueSettings.Instance.WorkQueueMaxFailureCount)
                    {
                        columns.Failed = true;
                    }
                    else
                    {
                        columns.FailureCount = sop.FailureCount++;
                    }
                }

                uidUpdateBroker.Update(sop.GetKey(), columns);
                updateContext.Commit();
            }
        }

        private static void DeleteUid(WorkQueueUid sop)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueUidEntityBroker delete = updateContext.GetBroker<IWorkQueueUidEntityBroker>();

                delete.Delete(sop.GetKey());

                updateContext.Commit();
            }
        }


        private string GetUidPath(WorkQueueUid sop)
        {
            string imagePath = Path.Combine(_context.ReconcileWorkQueueData.StoragePath, sop.SopInstanceUid + ".dcm");
            return imagePath;
        }

        private void PrepareWorkingFolder()
        {
            Directory.CreateDirectory(_workingDir);

            foreach (WorkQueueUid uid in _context.WorkQueueUidList)
            {
                string imagePath = GetUidPath(uid);
                string destPath = Path.Combine(_workingDir, uid.SopInstanceUid + ".dcm");
                File.Copy(imagePath, destPath);

                _fileToUidMap.Add(destPath, uid);
            }


            FileProcessor.Process(_workingDir, "*.dcm",
                                  delegate(string path)
                                      {
                                          DicomFile file = new DicomFile(path);
                                          file.Load(DicomReadOptions.StorePixelDataReferences);
                                          Platform.Log(LogLevel.Info, "Processing {0}", path);
                                          foreach (BaseImageLevelUpdateCommand command in _imageLevelCommands)
                                          {
                                              command.File = file;
                                              command.Execute();
                                          }

                                          // work around a bug in dicom toolkit
                                          // Can't overwrite the file that's opened by Load(DicomReadOptions.StorePixelDataReferences)
                                          file.Save(path + ".temp");
                                          File.Delete(path);
                                          File.Move(path + ".temp", path);

                                      }, true);
        }

        #endregion
        
        #region IReconcileServerCommand Members

        public ReconcileStudyProcessorContext Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }

        
        public IEnumerable<WorkQueueUid> ProcessedUidList
        {
            get { return _processedUidList; }
        }

        public IEnumerable<WorkQueueUid> FailedUidList
        {
            get { return _failedUidList; }
        }

        public StudyStorageLocation DestStudyStorage
        {
            get { return _destStudyStorage; }
            set { _destStudyStorage = value; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            CleanupWorkingFolder();

            if (_processor != null)
            {
                _processor.Dispose();
                _processor = null;
            }
        }

        #endregion

        public void SetContext(ReconcileStudyProcessorContext context)
        {
            _context = context;
        }
    }
}