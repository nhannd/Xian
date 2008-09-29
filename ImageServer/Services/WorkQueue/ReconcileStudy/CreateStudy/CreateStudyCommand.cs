using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
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
using ClearCanvas.ImageServer.Services.Dicom;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy
{
    
    class CreateStudyCommandXmlParser
    {
        public IList<IImageLevelUpdateCommand> ParseImageLevelCommands(XmlNode createStudyNode)
        {
            List<IImageLevelUpdateCommand> _commands = new List<IImageLevelUpdateCommand>();

            foreach (XmlNode subNode in createStudyNode.ChildNodes)
            {
                if (!(subNode is XmlComment))
                {
                    if (subNode.Name == "SetTag")
                    {
                        UpdateTagCommandParser parser = new UpdateTagCommandParser();
                        _commands.Add(parser.Parse(subNode));
                    }
                    else
                    {
                        throw new NotSupportedException(subNode.Name);
                    }
                }
            }

            return _commands;
        }
    }

    

    class CreateStudyCommand : ServerCommand, IDisposable
    {
        private ReconcileStudyProcessorContext _context;
        private ServerCommandProcessor _processor;
        private readonly List<WorkQueueUid> _processedUidList = new List<WorkQueueUid>();
        private readonly List<WorkQueueUid> _failedUidList = new List<WorkQueueUid>();
        private readonly List<WorkQueueUid> _duplicateList = new List<WorkQueueUid>(); 

        private readonly Dictionary<string, WorkQueueUid> _fileToUidMap = new Dictionary<string, WorkQueueUid>();

        private readonly string _workingDir = ServerPlatform.GetTempPath();

        private readonly List<IImageLevelUpdateCommand> _imageLevelCommands = new List<IImageLevelUpdateCommand>();
        private StudyStorageLocation _destStudyStorage = null;
        
        public CreateStudyCommand()
            : base("Create Study", true)
        {
            
        }

        public List<IImageLevelUpdateCommand> ImageLevelCommands
        {
            get
            {
                return _imageLevelCommands;
            }
        }

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
            foreach (IImageLevelUpdateCommand cmd in _imageLevelCommands)
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

        public StudyStorageLocation GetStudyStorageLocation(DicomFile file)
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
                    Filesystem filesystem = selector.SelectFilesystem(file);
                    if (filesystem == null)
                    {
                        Platform.Log(LogLevel.Error, "Unable to select location for storing study.");

                        return null;
                    }

                    IInsertStudyStorage locInsert = store.OpenReadContext().GetBroker<IInsertStudyStorage>();
                    StudyStorageInsertParameters insertParms = new StudyStorageInsertParameters();
                    insertParms.ServerPartitionKey = _context.Partition.GetKey();
                    insertParms.StudyInstanceUid = studyInstanceUid;
                    insertParms.Folder = studyDate;
                    insertParms.FilesystemKey = filesystem.GetKey();

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
            bool dupImage = false;

            ServerCommandProcessor processor = new ServerCommandProcessor("Reconciling image processor");
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
            processor.AddCommand(new UpdateWorkQueueCommand(file, destStudyStorage, dupImage, extension));
            
            if (!processor.Execute())
            {
                FailUid(uid, true);
                throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", file.Filename, processor.FailureReason));
            }

            if (_fileToUidMap.ContainsKey(workingImagePath))
            {
                DeleteUid(uid);
                File.Delete(GetUidPath(uid));
                _processedUidList.Add(uid);
            }

            
        }

        void FailDuplicate(WorkQueueUid sop)
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

        private void FailUid(WorkQueueUid sop, bool retry)
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

        private void DeleteUid(WorkQueueUid sop)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueUidEntityBroker delete = updateContext.GetBroker<IWorkQueueUidEntityBroker>();

                delete.Delete(sop.GetKey());

                updateContext.Commit();
            }
        }

        
        string GetUidPath(WorkQueueUid sop)
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
                                          foreach (IImageLevelUpdateCommand command in _imageLevelCommands)
                                          {
                                              command.Apply(file);
                                          }

                                          // work around a bug in dicom toolkit
                                          file.Save(path + ".temp");
                                          File.Delete(path);
                                          File.Move(path + ".temp", path);

                                      }, true);
        }


        protected override void OnUndo()
        {
            if (_processor!=null)
            {
                _processor.Rollback();
                _processor = null;
            }
        }

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
        }

        #endregion

        #region IReconcileServerCommand Members

        public void SetContext(ReconcileStudyProcessorContext context)
        {
            _context = context;
        }

        #endregion
    }
}