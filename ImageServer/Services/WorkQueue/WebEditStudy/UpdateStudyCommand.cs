using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Enumerated value for patient level update operation.
    /// </summary>
    enum UpdateMode
    {
        /// <summary>
        /// Update existing patient with new demographics 
        /// </summary>
        UpdatePatient,

        /// <summary>
        /// Create patient if neccessary
        /// </summary>
        CreatePatient,
    }

    class InstanceInfo
    {
        private string _seriesInstanceUid;
        private string _sopInstanceUid;

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public string SopInstanceUid
        {
            get { return _sopInstanceUid; }
            set { _sopInstanceUid = value; }
        }
    }

    /// <summary>
    /// Command for updating a study.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// 
    class UpdateStudyCommand : ServerDatabaseCommand, IDisposable
    {
        #region Private Members
        private readonly List<InstanceInfo> _updatedSopList = new List<InstanceInfo>();
        private readonly StudyStorageLocation _studyLocation;
        private readonly string _oldPatientId;
        private readonly string _oldPatientIdIssuer;
        private readonly string _newPatientId;
        private readonly string _newPatientIdIssuer;
        private readonly string _oldStudyPath;
        private readonly string _oldStudyInstanceUid;
        private readonly string _newStudyFolder;
        private readonly string _newStudyInstanceUid;
        private readonly string _oldStudyFolder;
        private readonly IList<IImageLevelUpdateCommand> _commands;
        private readonly string _newStudyPath;
        private readonly string _backupDir = ServerPlatform.GetTempPath();
        private readonly ServerPartition _partition;
        private Study _study;
        private Patient _patient;
        private StudyStorage _storage;
        private bool _originalFolderDeleted = false;
        private UpdateMode _mode = UpdateMode.UpdatePatient;
        #endregion

        #region Constructors
        public UpdateStudyCommand(ServerPartition partition, 
                                  StudyStorageLocation studyLocation,
                                  IList<IImageLevelUpdateCommand> imageLevelCommands) 
            : base("Update existing study", true)
        {
            _partition = partition;
            _studyLocation = studyLocation;
            _commands = imageLevelCommands;
            _oldStudyPath = studyLocation.GetStudyPath();
            _oldStudyInstanceUid = studyLocation.StudyInstanceUid;
            _oldStudyFolder = studyLocation.StudyFolder;
            _newStudyFolder = _oldStudyFolder;
            _newStudyInstanceUid = _oldStudyInstanceUid;

            _study = Study.Find(_oldStudyInstanceUid, _partition);
            _oldPatientId = _study.PatientId;
            _newPatientId = _oldPatientId;
            _oldPatientIdIssuer = _study.IssuerOfPatientId;
            _newPatientIdIssuer = _oldPatientIdIssuer;

            foreach (IImageLevelUpdateCommand command in imageLevelCommands)
            {
                if (command is IUpdateImageTagCommand)
                {
                    ImageLevelUpdateEntry imageLevelUpdate = (command as IUpdateImageTagCommand).UpdateEntry;
                    if (imageLevelUpdate != null)
                    {
                        if (imageLevelUpdate.Tag.TagValue == DicomTags.StudyDate)
                        {
                            _newStudyFolder = imageLevelUpdate.GetStringValue();
                        }
                        else if (imageLevelUpdate.Tag.TagValue == DicomTags.StudyInstanceUid)
                        {
                            _newStudyInstanceUid = imageLevelUpdate.GetStringValue();
                        }
                        else if (imageLevelUpdate.Tag.TagValue == DicomTags.PatientId)
                        {
                            _newPatientId = imageLevelUpdate.GetStringValue();
                        }
                        else if (imageLevelUpdate.Tag.TagValue == DicomTags.IssuerOfPatientId)
                        {
                            _newPatientIdIssuer = imageLevelUpdate.GetStringValue();
                        }
                    }
                }

            }

            Platform.CheckForNullReference(_newStudyInstanceUid, "_newStudyInstanceUid");

            if (String.IsNullOrEmpty(_newStudyFolder))
            {
                _newStudyFolder = ImageServerCommonConfiguration.DefaultStudyRootFolder;
            }

            _newStudyPath = Path.Combine(studyLocation.FilesystemPath, partition.PartitionFolder);
            _newStudyPath = Path.Combine(_newStudyPath, _newStudyFolder);
            _newStudyPath = Path.Combine(_newStudyPath, _newStudyInstanceUid);
        }

        public UpdateMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        #endregion

        #region Private Methods

        private void DetectUseCase()
        {
            if (_newPatientId!=_oldPatientId)
            {
                switch (_mode)
                {
                    case UpdateMode.CreatePatient:
                        throw new NotSupportedException("Not supported study edit use case: Create study for a new patient");

                    case UpdateMode.UpdatePatient:
                        {
                            Patient curPatient = FindPatient(_oldPatientId, _oldPatientIdIssuer);
                            Patient newPatient = FindPatient(_newPatientId, _newPatientIdIssuer);
                            

                            if (newPatient==null)
                            {
                                // new patient is not in the system, we are trying to udpate the current patient with new patient demographics
                                Platform.Log(LogLevel.Info, "Use case: Update study and current patient demographics");
                            }
                            else
                            {
                                Debug.Assert(newPatient != curPatient); // it can't be the same patient since the id is didfferent

                                // We are trying to append the study to an existing patient.
                                throw new NotSupportedException("Not supported study edit use case: Append study to a different patient");
                            }

                            break;
                        }
                }
               
            }
            else
            {
                Platform.Log(LogLevel.Info, "Use case: Update study of the current patient");
            }
        }

        private Patient FindPatient(string id, string issuer)
        {
            IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
            IPatientEntityBroker patientFindBroker = readContext.GetBroker<IPatientEntityBroker>();
            PatientSelectCriteria criteria = new PatientSelectCriteria();
            criteria.PatientId.EqualTo(id);

            if (String.IsNullOrEmpty(issuer))
                criteria.IssuerOfPatientId.IsNull();
            else
                criteria.IssuerOfPatientId.EqualTo(issuer);

            return patientFindBroker.FindOne(criteria);
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            PrintUpdateCommands();
            DetectUseCase();
            BackupFilesystem();
            UpdateFilesystem();
            UpdateDatabase();
        }

        protected override void OnUndo()
        {
            RestoreFilesystem();

            // db rollback is done by the processor
        }

        private void PrintUpdateCommands()
        {
            StringBuilder log = new StringBuilder();
            log.AppendFormat("Study to be updated:\n");
            log.AppendFormat("\tServer Partition: {0}\n", _partition.AeTitle);
            log.AppendFormat("\tStorage GUID: {0}\n", _studyLocation.GetKey().Key);
            log.AppendFormat("\tPatient ID: {0}\n", _study.PatientId);
            log.AppendFormat("\tPatient Name: {0}\n", _study.PatientsName);
            log.AppendFormat("\tAccession #: {0}\n", _study.AccessionNumber);
            log.AppendFormat("\tStudy ID : {0}\n", _study.StudyId);
            log.AppendFormat("\tStudy Date : {0}\n", _study.StudyDate);
            log.AppendFormat("\tStudy Instance Uid: {0}\n", _study.StudyInstanceUid);
            log.AppendLine();
            log.AppendFormat("Changes:\n");
            foreach (IImageLevelUpdateCommand cmd in _commands)
            {
                log.AppendFormat("\t{0}", cmd);
                log.AppendLine();
            }

            log.AppendFormat("New Study Location: {0}", _newStudyPath);

            Platform.Log(LogLevel.Info, log);
        }
        private void RestoreFilesystem()
        {
            if (_newStudyPath == _oldStudyPath)
            {
                Platform.Log(LogLevel.Info, "Restoring old study folder...");
            
                // files were overwritten
                if (Directory.Exists(_oldStudyPath))
                    Directory.CreateDirectory(_oldStudyPath);

                // restore header
                Platform.Log(LogLevel.Info, "Restoring old study header...");
                string backupFilePath = Path.Combine(_backupDir, _oldStudyInstanceUid +".xml" );
                File.Copy(backupFilePath, Path.Combine(_oldStudyPath, _oldStudyInstanceUid + ".xml"), true);
                backupFilePath = Path.Combine(_backupDir, _oldStudyInstanceUid + ".xml.gz");
                File.Copy(backupFilePath, Path.Combine(_oldStudyPath, _oldStudyInstanceUid + ".xml.gz"), true);

                // restore updated SOPs
                foreach (InstanceInfo sop in _updatedSopList)
                {
                    string backupUidPath = Path.Combine(_backupDir, sop.SeriesInstanceUid);
                    backupUidPath = Path.Combine(backupUidPath,  sop.SopInstanceUid +".dcm");
                    DicomFile file = new DicomFile(backupUidPath);
                    file.Load();

                    string seriesUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                    string sopUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                    string oldPath = Path.Combine(_oldStudyPath, seriesUid);
                    if (Directory.Exists(oldPath))
                        Directory.CreateDirectory(oldPath);

                    oldPath = Path.Combine(oldPath, sopUid + ".dcm");

                    Platform.Log(LogLevel.Info, "Restoring SOP {0}", sopUid);
                    file.Save(oldPath);
                }
            }
            else
            {
                DirectoryUtility.DeleteIfExists(_newStudyPath, true);

                if (_originalFolderDeleted)
                {
                    // the old folder was removed. It must be restored entirely
                    Platform.Log(LogLevel.Info, "Restoring original study folder");
                    DirectoryUtility.Copy(_backupDir, _oldStudyPath);
                }
                
            }
        }

        private void UpdateEntity(ServerEntity entity)
        {
            EntityDicomMap entityMap = EntityDicomMapManager.Get(entity.GetType());

            foreach (IImageLevelUpdateCommand command in _commands)
            {
                if (command is IUpdateImageTagCommand)
                {
                    ImageLevelUpdateEntry entry = (command as IUpdateImageTagCommand).UpdateEntry;
                    if (entityMap.ContainsKey(entry.Tag))
                    {
                        entityMap[entry.Tag].SetValue(entity, entry.GetStringValue(), null);
                    }
                }

            }

        }

        private void LoadEntities()
        {
            _storage = StudyStorage.Load(_studyLocation.GetKey());
            _study = Study.Find(_studyLocation.StudyInstanceUid, _partition);
            _patient = Patient.Load(_study.PatientKey);
        }

        private void UpdateDatabase()
        {
            LoadEntities();

            UpdateEntity(_study);
            UpdateEntity(_patient);
            UpdateEntity(_storage);

            IStudyEntityBroker studyUpdateBroker = UpdateContext.GetBroker<IStudyEntityBroker>();
            studyUpdateBroker.Update(_study);

            IPatientEntityBroker patientUpdateBroker = UpdateContext.GetBroker<IPatientEntityBroker>();
            patientUpdateBroker.Update(_patient);

            IStudyStorageEntityBroker storageUpdateBroker = UpdateContext.GetBroker<IStudyStorageEntityBroker>();
            storageUpdateBroker.Update(_storage);

            IFilesystemStudyStorageEntityBroker filesystemStorageBroker = UpdateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
            FilesystemStudyStorageSelectCriteria criteria = new FilesystemStudyStorageSelectCriteria();
            criteria.FilesystemKey.Equals(_studyLocation.FilesystemKey);
            criteria.StudyStorageKey.EqualTo(_studyLocation.GetKey());
            FilesystemStudyStorageUpdateColumns columns = new FilesystemStudyStorageUpdateColumns();
            columns.StudyFolder = _newStudyFolder;
            filesystemStorageBroker.Update(criteria, columns);
        }

        private void UpdateFilesystem()
        {
            Platform.Log(LogLevel.Info, "Updating filesystem...");
            string studyXmlPath = Path.Combine(_oldStudyPath, _oldStudyInstanceUid + ".xml"); ;
            StudyXml studyXml = LoadStudyXml(studyXmlPath);

            StudyXml newStudyXml = new StudyXml();
            foreach (SeriesXml seriesXml in studyXml)
            {
                foreach (InstanceXml instanceXml in seriesXml)
                {
                    string path = Path.Combine(_oldStudyPath, seriesXml.SeriesInstanceUid);
                    path = Path.Combine(path, instanceXml.SopInstanceUid);
                    path += ".dcm";

                    //create backup
                    try
                    {
                        DicomFile file = new DicomFile(path);
                        file.Load();

                        InstanceInfo instance = new InstanceInfo();
                        instance.SeriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                        instance.SopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                        
                        foreach (IImageLevelUpdateCommand command in _commands)
                        {
                            command.Apply(file);
                        }

                        SaveFile(file);

                        _updatedSopList.Add(instance);

                        newStudyXml.AddFile(file);
                    }
                    catch (Exception)
                    {
                        File.Delete(Path.Combine(_backupDir, instanceXml.SopInstanceUid) + ".bak"); //dont' need to restore this file
                        throw;
                    }

                }
            }

            Platform.Log(LogLevel.Info, "Generating new study header...");
            string newStudyXmlPath = Path.Combine(_newStudyPath, _newStudyInstanceUid + ".xml");
            string gzipStudyXmlPath = Path.Combine(_newStudyPath, _newStudyInstanceUid + ".xml.gz");
            using (FileStream xmlStream = new FileStream(newStudyXmlPath, FileMode.Create),
                              gzipStream = new FileStream(gzipStudyXmlPath, FileMode.Create))
            {
                StudyXmlIo.WriteXmlAndGzip(newStudyXml.GetMemento(new StudyXmlOutputSettings()), xmlStream, gzipStream);
            }

            if (_newStudyPath!=_oldStudyPath)
            {
                Platform.Log(LogLevel.Info, "Removing old study folder...");
                DirectoryUtility.DeleteIfExists(_oldStudyPath, true);
                _originalFolderDeleted = true;
            }
        }

        private void SaveFile(DicomFile file)
        {
            String seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            String sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

            String destPath = _studyLocation.FilesystemPath;
            String extension = ".dcm";

            ServerCommandProcessor filesystemUpdateProcessor = new ServerCommandProcessor("Filesystem update processor");
            filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, _partition.PartitionFolder);
            filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, _newStudyFolder);
            filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, _newStudyInstanceUid);
            filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, seriesInstanceUid);
            filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, sopInstanceUid);
            destPath += extension;

            if (File.Exists(destPath))
            {
                // overwrite it
            }

            SaveDicomFileCommand saveCommand = new SaveDicomFileCommand(destPath, file);
            filesystemUpdateProcessor.AddCommand(saveCommand);

            if (!filesystemUpdateProcessor.Execute())
            {
                throw new ApplicationException(String.Format("Unable to update image {0} : {1}", file.Filename, filesystemUpdateProcessor.FailureReason));
            }

            Platform.Log(LogLevel.Info, "SOP {0} updated.", sopInstanceUid);
        }

        private void BackupFilesystem()
        {
            Directory.CreateDirectory(_backupDir);
            DirectoryUtility.Copy(_oldStudyPath, _backupDir);
        }

        private StudyXml LoadStudyXml(string studyXmlPath)
        {
            XmlDocument doc = new XmlDocument();
            StudyXml studyXml = new StudyXml();
            using (FileStream stream = File.OpenRead(studyXmlPath))
            {
                StudyXmlIo.Read(doc, stream);
                studyXml.SetMemento(doc);
            }

            return studyXml;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            DirectoryUtility.DeleteIfExists(_backupDir);
        }

        #endregion
    }

}