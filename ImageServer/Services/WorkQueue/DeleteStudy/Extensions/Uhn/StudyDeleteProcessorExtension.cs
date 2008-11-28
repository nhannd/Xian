using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions.Uhn
{
    public class StudyDeleteExtendedInfo
    {
        private string _serverInstanceId;

        /// <summary>
        /// The server which processed the study delete request.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string ServerInstanceId
        {
            get { return _serverInstanceId; }
            set { _serverInstanceId = value; }
        }
    }

    
    [ExtensionOf(typeof(DeleteStudyProcessorExtensionPoint))]
    internal class StudyDeleteProcessorExtension:IDeleteStudyProcessorExtension
    {
        private DeleteStudyContext _context;
        private bool _enabled = true;
        private string _backupPath;
        
        #region IDeleteStudyProcessorExtension Members

        public bool Enabled
        {
            get { return _enabled; }
        }

        #endregion

        private string BackupSubPath
        {
            get
            {
                if (String.IsNullOrEmpty(_backupPath))
                {

                    string pathPrefix = Path.Combine("Deleted", _context.ServerPartition.PartitionFolder);

                    _backupPath = Path.Combine(pathPrefix, _context.Study.StudyInstanceUid);

                    int counter = 1;
                    while (Directory.Exists(_context.Filesystem.ResolveAbsolutePath(_backupPath)))
                    {
                        _backupPath = Path.Combine(pathPrefix, String.Format("{0}({1})", _context.Study.StudyInstanceUid, counter++));
                    }
                }

                return _backupPath;
            }
        }
        

        #region IDeleteStudyProcessorExtension Members

        public void Initialize(DeleteStudyContext context)
        {
            Platform.CheckForNullReference(context, "context");
            Platform.CheckForNullReference(context.Study, "context.Study");
            _context = context;

            // TODO: Load from configuration
            _enabled = true;
        }

        public void OnStudyDeleting()
        {
            // only backup if study is manually deleted
            if (_context.WorkQueueItem.WorkQueueTypeEnum == WorkQueueTypeEnum.WebDeleteStudy)
            {
                StudyStorageLocation storage = _context.StorageLocation;
                using (ServerCommandProcessor processor = new ServerCommandProcessor("Backup deleted study"))
                {
                    string path = _context.Filesystem.ResolveAbsolutePath(BackupSubPath);

                    Platform.Log(LogLevel.Info, "Backing up study to {0}...", path);

                    CreateDirectoryCommand mkdir = new CreateDirectoryCommand(path);
                    processor.AddCommand(mkdir);

                    string zipStudyBackupPath = Path.Combine(path, _context.Study.StudyInstanceUid + ".zip");
                    ZipStudyFolderCommand zip = new ZipStudyFolderCommand(storage.GetStudyPath(), zipStudyBackupPath);
                    processor.AddCommand(zip);

                    if (!processor.Execute())
                    {
                        throw new ApplicationException(String.Format("Unable to backup study: {0}", processor.FailureReason));
                    }
                }
            }
            
        }

        public void OnStudyDeleted()
        {
            if (_context.WorkQueueItem.WorkQueueTypeEnum == WorkQueueTypeEnum.WebDeleteStudy)
            {
                Study study = _context.Study;
                StudyStorageLocation storage = _context.StorageLocation;

                using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    // Setup the parameters
                    IStudyDeleteRecordEntityBroker broker = updateContext.GetBroker<IStudyDeleteRecordEntityBroker>();

                    StudyDeleteRecordUpdateColumns parms = new StudyDeleteRecordUpdateColumns();
                    parms.Timestamp = Platform.Time;
                    parms.Reason = _context.WorkQueueItem.WorkQueueTypeEnum.LongDescription;
                    parms.ServerPartitionKey = _context.ServerPartition.GetKey();
                    parms.FilesystemKey = storage.FilesystemKey;

                    parms.AccessionNumber = study.AccessionNumber;
                    parms.PatientId = study.PatientId;
                    parms.PatientsName = study.PatientsName;
                    parms.StudyInstanceUid = study.StudyInstanceUid;
                    parms.StudyDate = study.StudyDate;
                    parms.StudyDescription = study.StudyDescription;
                    parms.StudyTime = study.StudyTime;

                    parms.BackupPath = BackupSubPath;

                    ArchiveStudyStorage archive = StudyStorageLocation.GetArchiveLocation(storage.GetKey());
                    if (archive != null)
                    {
                        parms.ArchiveStorageKey = archive.GetKey();
                    }

                    StudyDeleteExtendedInfo extInfo = new StudyDeleteExtendedInfo();
                    extInfo.ServerInstanceId = ServiceTools.ServerInstanceId;
                    parms.ExtendedInfo = XmlUtils.SerializeAsString(extInfo);

                    StudyDeleteRecord deleteRecord = broker.Insert(parms);
                    if (deleteRecord == null)
                    {
                        Platform.Log(LogLevel.Error, "Unexpected error when trying to create study delete record: {0} on partition {1}",
                                     study.StudyInstanceUid, _context.ServerPartition.Description);
                    }
                    else
                        updateContext.Commit();
                }
            }

            
        }

        #endregion

        
    }
}