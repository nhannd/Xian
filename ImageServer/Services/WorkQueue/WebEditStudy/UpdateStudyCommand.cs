using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{


    class PatientInfo : IEquatable<PatientInfo>
    {
        private string _name;
        private string _patientId;
        private string _issuerOfPatientId;

        public PatientInfo()
        {
        }

        public PatientInfo(PatientInfo other)
        {
            Name = other.Name;
            PatientId = other.PatientId;
            IssuerOfPatientId = other.IssuerOfPatientId;
        }
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public string IssuerOfPatientId
        {
            get { return _issuerOfPatientId; }
            set { _issuerOfPatientId = value; }
        }

        #region IEquatable<PatientInfo> Members

        public bool Equals(PatientInfo other)
        {
            PersonName name = new PersonName(_name);
            PersonName otherName = new PersonName(other.Name);
            return name.Equals(otherName) && String.Equals(_patientId, other.PatientId, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
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
        private readonly string _oldStudyPath;
        private readonly string _oldStudyInstanceUid;
        private readonly string _newStudyFolder;
        private readonly string _newStudyInstanceUid;
        private readonly string _oldStudyFolder;

        private PatientInfo _oldPatientInfo;
        private PatientInfo _newPatientInfo;

        private readonly IList<IImageLevelUpdateCommand> _commands;
        private readonly string _newStudyPath;
        private readonly string _backupDir = ServerPlatform.GetTempPath();
        private readonly ServerPartition _partition;
        private Study _study;
        private Patient _curPatient;
        private Patient _newPatient;
        private StudyStorage _storage;
        private bool _originalFolderDeleted = false;

        private bool _patientInfoIsNotChanged;

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
            
            _curPatient = Patient.Load(_study.PatientKey);

            _oldPatientInfo = new PatientInfo();
            _oldPatientInfo.Name = _curPatient.PatientsName;
            _oldPatientInfo.PatientId = _curPatient.PatientId;
            _oldPatientInfo.IssuerOfPatientId = _curPatient.IssuerOfPatientId;

            _newPatientInfo = new PatientInfo(_oldPatientInfo);
            Debug.Assert(_newPatientInfo.Equals(_oldPatientInfo));

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
                            _newPatientInfo.PatientId = imageLevelUpdate.GetStringValue();
                        }
                        else if (imageLevelUpdate.Tag.TagValue == DicomTags.IssuerOfPatientId)
                        {
                            _newPatientInfo.IssuerOfPatientId = imageLevelUpdate.GetStringValue();
                        }
                        else if (imageLevelUpdate.Tag.TagValue == DicomTags.PatientsName)
                        {
                            _newPatientInfo.Name = imageLevelUpdate.GetStringValue();
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

            _newPatient = FindPatient(_newPatientInfo);
            _patientInfoIsNotChanged = _newPatientInfo.Equals(_oldPatientInfo);
        }
        #endregion

        #region Private Methods

        private Patient FindPatient(PatientInfo patientInfo)
        {
            IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
            IPatientEntityBroker patientFindBroker = readContext.GetBroker<IPatientEntityBroker>();
            PatientSelectCriteria criteria = new PatientSelectCriteria();
            criteria.PatientId.EqualTo(patientInfo.PatientId);
            criteria.PatientsName.EqualTo(patientInfo.Name);

            return patientFindBroker.FindOne(criteria);
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            PrintUpdateCommands();
            BackupFilesystem();
            UpdateFilesystem();
            UpdateDatabase();

            Rearchive();
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
        }

        private void UpdateDatabase()
        {
            LoadEntities();

            UpdateEntity(_study);
            UpdateEntity(_curPatient);
            UpdateEntity(_storage);

            // Update the Study table
            IStudyEntityBroker studyUpdateBroker = UpdateContext.GetBroker<IStudyEntityBroker>();
            studyUpdateBroker.Update(_study);
            
            // Update the StudyStorage table
            IStudyStorageEntityBroker storageUpdateBroker = UpdateContext.GetBroker<IStudyStorageEntityBroker>();
            storageUpdateBroker.Update(_storage);

            // Update the FilesystemStudyStorage table
            IFilesystemStudyStorageEntityBroker filesystemStorageBroker = UpdateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
            FilesystemStudyStorageSelectCriteria criteria = new FilesystemStudyStorageSelectCriteria();
            criteria.FilesystemKey.Equals(_studyLocation.FilesystemKey);
            criteria.StudyStorageKey.EqualTo(_studyLocation.GetKey());
            FilesystemStudyStorageUpdateColumns columns = new FilesystemStudyStorageUpdateColumns();
            columns.StudyFolder = _newStudyFolder;
            filesystemStorageBroker.Update(criteria, columns);

            // Update Patient level info. Different cases can occur here: 
            //      A) Patient demographic info is not changed ==> update the current patient
            //      B) New patient demographics matches (another) existing patient in the datbase 
            //              ==> Transfer the study to that patient. This means the study count on both patients must be updated.
            //                  The current patient should also be deleted if there's no more study attached to it after the transfer.
            //      C) New patient demographics doesn't match any patient in the database
            //              ==> A new patient should be created for this study. The study count on the current patient should be updated
            //                  and the patient should also be deleted if this is the only study attached to it.
            if (_patientInfoIsNotChanged)
            {
                UpdateCurrentPatient(_study, _curPatient);
            }
            else 
            {
                if (_newPatient == null) 
                {
                    // No matching patient in the database. We should create a new patient for this study
                    _newPatient = CreateNewPatient(_study, _newPatientInfo); 
                }
                else
                {
                    // There's already patient in the database with the new patient demographics
                    // The study should be attached to that patient.
                    TransferStudy(_study, _oldPatientInfo, _newPatient);
                }
            }


        }

        private void Rearchive()
        {
            Platform.Log(LogLevel.Info, "Scheduling/Update study archive..");
            Platform.Log(LogLevel.Info, "This feature is not implemented.");

            // TODO: call stored procedure to trigger the archive
        }

        private Patient CreateNewPatient(Study study, PatientInfo patientInfo)
        {
            Platform.Log(LogLevel.Info, "Creating new patient {0}", patientInfo.PatientId);
            ICreatePatientForStudy createStudyBroker = UpdateContext.GetBroker<ICreatePatientForStudy>();
            CreatePatientForStudyParameters parms = new CreatePatientForStudyParameters();
            parms.IssuerOfPatientId = _newPatientInfo.IssuerOfPatientId;
            parms.PatientId = _newPatientInfo.PatientId;
            parms.PatientsName = _newPatientInfo.Name;
            //parms.SpecificCharacterSet = null;
            parms.StudyKey = _study.GetKey();
            Patient newPatient = createStudyBroker.FindOne(parms);
            if (newPatient==null)
                throw new ApplicationException("Unable to create patient for the study");

            return newPatient;
        }

        private void UpdateCurrentPatient(Study study, Patient patient)
        {
            Platform.Log(LogLevel.Info, "Update current patient record");
            IPatientEntityBroker patientUpdateBroker = UpdateContext.GetBroker<IPatientEntityBroker>();
            patientUpdateBroker.Update(_curPatient);
        }

        private void TransferStudy(Study study, PatientInfo oldPatient, Patient newPatient)
        {
            Platform.Log(LogLevel.Info, "Transferring study from {0} [ID={1}] to {2} [ID={3}]",
                        oldPatient.Name, oldPatient.PatientId, newPatient.PatientsName, newPatient.PatientId);

            IAttachStudyToPatient attachStudyToPatientBroker = UpdateContext.GetBroker<IAttachStudyToPatient>();
            AttachStudyToPatientParamaters parms = new AttachStudyToPatientParamaters();
            parms.StudyKey = study.GetKey();
            parms.NewPatientKey = newPatient.GetKey();
            attachStudyToPatientBroker.Execute(parms);
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