using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
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
    /// Stores statistics of a WorkQueue instance processing.
    /// </summary>
    internal class UpdateStudyStatistics : StatisticsSet
    {
        #region Constructors

        public UpdateStudyStatistics(string studyInstanceUid)
            : this("UpdateStudy", studyInstanceUid)
        { }

        public UpdateStudyStatistics(string name, string studyInstanceUid)
            : base(name)
        {
            AddField("StudyInstanceUid", studyInstanceUid);
        }

        #endregion Constructors

        #region Public Properties

        
        public TimeSpanStatistics ProcessTime
        {
            get
            {
                if (this["ProcessTime"] == null)
                    this["ProcessTime"] = new TimeSpanStatistics("ProcessTime");

                return (this["ProcessTime"] as TimeSpanStatistics);
            }
            set { this["ProcessTime"] = value; }
        }

        public ulong StudySize
        {
            set
            {
                this["StudySize"] = new ByteCountStatistics("StudySize", value);
            }
            get
            {
                if (this["StudySize"] == null)
                    this["StudySize"] = new ByteCountStatistics("StudySize");

                return ((ByteCountStatistics)this["StudySize"]).Value;
            }
        }

        public int InstanceCount
        {
            set
            {
                this["InstanceCount"] = new Statistics<int>("InstanceCount", value);
            }
            get
            {
                if (this["InstanceCount"] == null)
                    this["InstanceCount"] = new Statistics<int>("InstanceCount");

                return ((Statistics<int>)this["InstanceCount"]).Value;
            }
        }

        #endregion Public Properties
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
        private string _oldStudyPath;
        private string _oldStudyInstanceUid;
        private string _newStudyFolder;
        private string _newStudyInstanceUid;
        private string _oldStudyFolder;
        private bool _initialized=false;

        private PatientInfo _oldPatientInfo;
        private PatientInfo _newPatientInfo;

        private readonly IList<BaseImageLevelUpdateCommand> _commands;
        private string _newStudyPath;
        private readonly string _backupDir = ServerPlatform.GetTempPath();
        private readonly ServerPartition _partition;
        private Study _study;
        private Patient _curPatient;
        private Patient _newPatient;
        private StudyStorage _storage;
        private bool _originalFolderDeleted = false;

        private bool _patientInfoIsNotChanged;

        private UpdateStudyStatistics _statistics;
        private int _totalSopCount;
        #endregion

        #region Constructors
        public UpdateStudyCommand(ServerPartition partition, 
                                  StudyStorageLocation studyLocation,
                                  IList<BaseImageLevelUpdateCommand> imageLevelCommands) 
            : base("Update existing study", true)
        {
            _partition = partition;
            _studyLocation = studyLocation;
            _commands = imageLevelCommands;
            _statistics = new UpdateStudyStatistics(_studyLocation.StudyInstanceUid);

        }

        public new UpdateStudyStatistics Statistics
        {
            get { return _statistics; }
        }

        public string NewStudyPath
        {
            get { return _newStudyPath; }
            set { _newStudyPath = value; }
        }

        #endregion

        #region Protected Method
        protected override void OnExecute(IUpdateContext updateContext)
        {
            Statistics.ProcessTime.Start();
            Initialize();
            PrintUpdateCommands();
            if (RequiresRollback)
            {
                BackupFilesystem();
            }
            UpdateFilesystem();
            UpdateDatabase();

            Statistics.ProcessTime.End();
        }

        protected override void OnUndo()
        {
            RestoreFilesystem();

            // db rollback is done by the processor
        }


        #endregion

        #region Private Methods
        private void Initialize()
        {

            _oldStudyPath = _studyLocation.GetStudyPath();
            _oldStudyInstanceUid = _studyLocation.StudyInstanceUid;
            _oldStudyFolder = _studyLocation.StudyFolder;
            _newStudyFolder = _oldStudyFolder;
            _newStudyInstanceUid = _oldStudyInstanceUid;

            _study = _studyLocation.Study;
            _totalSopCount = _study.NumberOfStudyRelatedInstances;
            _curPatient = _study.Patient;
            _oldPatientInfo = new PatientInfo();
            _oldPatientInfo.Name = _curPatient.PatientsName;
            _oldPatientInfo.PatientId = _curPatient.PatientId;
            _oldPatientInfo.IssuerOfPatientId = _curPatient.IssuerOfPatientId;

            _newPatientInfo = new PatientInfo(_oldPatientInfo);
            Debug.Assert(_newPatientInfo.Equals(_oldPatientInfo));

            foreach (BaseImageLevelUpdateCommand command in _commands)
            {
                if (command is IUpdateImageTagCommand)
                {
                    ImageLevelUpdateEntry imageLevelUpdate = (command as IUpdateImageTagCommand).UpdateEntry;
                    if (imageLevelUpdate != null)
                    {
                        if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.StudyDate)
                        {
                        	// Update the folder name if the system is not currently using receiving date as the study folder
                            if (!ImageServerCommonConfiguration.UseReceiveDateAsStudyFolder)
                                _newStudyFolder = imageLevelUpdate.GetStringValue();
                        }
                        else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.StudyInstanceUid)
                        {
                            _newStudyInstanceUid = imageLevelUpdate.GetStringValue();
                        }
                        else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.PatientId)
                        {
                            _newPatientInfo.PatientId = imageLevelUpdate.GetStringValue();
                        }
                        else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.IssuerOfPatientId)
                        {
                            _newPatientInfo.IssuerOfPatientId = imageLevelUpdate.GetStringValue();
                        }
                        else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.PatientsName)
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

            _newStudyPath = Path.Combine(_studyLocation.FilesystemPath, _partition.PartitionFolder);
            _newStudyPath = Path.Combine(_newStudyPath, _newStudyFolder);
            _newStudyPath = Path.Combine(_newStudyPath, _newStudyInstanceUid);

            _newPatient = FindPatient(_newPatientInfo);
            _patientInfoIsNotChanged = _newPatientInfo.Equals(_oldPatientInfo);

            Statistics.InstanceCount = _study.NumberOfStudyRelatedInstances;
            Statistics.StudySize = (ulong) DirectoryUtility.CalculateFolderSize(_oldStudyPath);

            _initialized = true;
        }

        private Patient FindPatient(PatientInfo patientInfo)
        {
            IPatientEntityBroker patientFindBroker = UpdateContext.GetBroker<IPatientEntityBroker>();
            PatientSelectCriteria criteria = new PatientSelectCriteria();
            criteria.PatientId.EqualTo(patientInfo.PatientId);
            criteria.PatientsName.EqualTo(patientInfo.Name);

            return patientFindBroker.FindOne(criteria);
        }

        private void PrintUpdateCommands()
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine(String.Format("Study to be updated:"));
            log.AppendLine(String.Format("\tServer Partition: {0}\n", _partition.AeTitle));
            log.AppendLine(String.Format("\tStorage GUID: {0}\n", _studyLocation.GetKey().Key));
            log.AppendLine(String.Format("\tPatient ID: {0}\n", _study.PatientId));
            log.AppendLine(String.Format("\tPatient Name: {0}\n", _study.PatientsName));
            log.AppendLine(String.Format("\tAccession #: {0}\n", _study.AccessionNumber));
            log.AppendLine(String.Format("\tStudy ID : {0}\n", _study.StudyId));
            log.AppendLine(String.Format("\tStudy Date : {0}\n", _study.StudyDate));
            log.AppendLine(String.Format("\tStudy Instance Uid: {0}\n", _study.StudyInstanceUid));
            log.AppendLine(String.Format("\tInstance Count: {0}\n", _study.NumberOfStudyRelatedInstances));
            log.AppendLine(String.Format("\tCurrent location: {0}", _oldStudyPath));
            log.AppendLine();
            log.AppendLine("Changes to be applied:");
            foreach (BaseImageLevelUpdateCommand cmd in _commands)
            {
                log.AppendLine(String.Format("\t{0}", cmd));
            }

            log.AppendLine(String.Format("\tNew location: {0}", NewStudyPath));
            Platform.Log(LogLevel.Info, log);
        }
        private void RestoreFilesystem()
        {
            if (!RequiresRollback || !_initialized)
                return;

            if (NewStudyPath == _oldStudyPath)
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
                DirectoryUtility.DeleteIfExists(NewStudyPath, true);

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

            foreach (BaseImageLevelUpdateCommand command in _commands)
            {
                if (command is IUpdateImageTagCommand)
                {
                    ImageLevelUpdateEntry entry = (command as IUpdateImageTagCommand).UpdateEntry;
                    if (entityMap.ContainsKey(entry.TagPath.Tag))
                    {
                        entityMap[entry.TagPath.Tag].SetValue(entity, entry.GetStringValue(), null);
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
                UpdateCurrentPatient();
            }
            else 
            {
                if (_newPatient == null) 
                {
                    // No matching patient in the database. We should create a new patient for this study
                    _newPatient = CreateNewPatient(_newPatientInfo); 
                }
                else
                {
                    // There's already patient in the database with the new patient demographics
                    // The study should be attached to that patient.
                    TransferStudy(_study, _oldPatientInfo, _newPatient);
                }
            }


            Rearchive();
        }

        private void Rearchive()
        {
            Platform.Log(LogLevel.Info, "Scheduling/Updating study archive..");
            _storage.Archive(UpdateContext);
        }

        private Patient CreateNewPatient(PatientInfo patientInfo)
        {
            Platform.Log(LogLevel.Info, "Creating new patient {0}", patientInfo.PatientId);
            ICreatePatientForStudy createStudyBroker = UpdateContext.GetBroker<ICreatePatientForStudy>();
            CreatePatientForStudyParameters parms = new CreatePatientForStudyParameters();
            parms.IssuerOfPatientId = _newPatientInfo.IssuerOfPatientId;
            parms.PatientId = _newPatientInfo.PatientId;
            parms.PatientsName = _newPatientInfo.Name;
            parms.SpecificCharacterSet = _curPatient.SpecificCharacterSet; // assume it's the same
            parms.StudyKey = _study.GetKey();
            Patient newPatient = createStudyBroker.FindOne(parms);
            if (newPatient==null)
                throw new ApplicationException("Unable to create patient for the study");

            return newPatient;
        }

        private void UpdateCurrentPatient()
        {
            Platform.Log(LogLevel.Info, "Update current patient record...");
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
                        
                        foreach (BaseImageLevelUpdateCommand command in _commands)
                        {
                            command.File = file;
                            command.Execute();
                        }

                        SaveFile(file);

                        _updatedSopList.Add(instance);

						long fileSize = 0;
						if (File.Exists(file.Filename))
						{
							FileInfo finfo = new FileInfo(file.Filename);

							fileSize = finfo.Length;
						}
                        newStudyXml.AddFile(file, fileSize);

                        Platform.Log(LogLevel.Info, "SOP {0} updated [{1} of {2}].", instance.SopInstanceUid, _updatedSopList.Count, _totalSopCount);
                    }
                    catch (Exception)
                    {
                        File.Delete(Path.Combine(_backupDir, instanceXml.SopInstanceUid) + ".bak"); //dont' need to restore this file
                        throw;
                    }

                }
            }

            if (_updatedSopList.Count != _totalSopCount)
            {
                Platform.Log(LogLevel.Warn, "Inconsistent data: expected {0} instances to be updated / Found {1}.", _totalSopCount, _updatedSopList.Count);
            }

            Platform.Log(LogLevel.Info, "Generating new study header...");
            string newStudyXmlPath = Path.Combine(NewStudyPath, _newStudyInstanceUid + ".xml");
            string gzipStudyXmlPath = Path.Combine(NewStudyPath, _newStudyInstanceUid + ".xml.gz");
            using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(newStudyXmlPath, FileMode.Create),
							  gzipStream = FileStreamOpener.OpenForSoleUpdate(gzipStudyXmlPath, FileMode.Create))
            {
                StudyXmlIo.WriteXmlAndGzip(newStudyXml.GetMemento(new StudyXmlOutputSettings()), xmlStream, gzipStream);
				xmlStream.Close();
				gzipStream.Close();
            }

            if (NewStudyPath!=_oldStudyPath)
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

            using (ServerCommandProcessor filesystemUpdateProcessor = new ServerCommandProcessor("Filesystem update processor"))
            {
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

                SaveDicomFileCommand saveCommand = new SaveDicomFileCommand(destPath, file, false);
                filesystemUpdateProcessor.AddCommand(saveCommand);

                if (!filesystemUpdateProcessor.Execute())
                {
                    throw new ApplicationException(String.Format("Unable to update image {0} : {1}", file.Filename, filesystemUpdateProcessor.FailureReason));
                }
            }
            
        }

        private void BackupFilesystem()
        {
            Platform.Log(LogLevel.Info, "Backing up current study folder...");
            Directory.CreateDirectory(_backupDir);
            DirectoryUtility.Copy(_oldStudyPath, _backupDir);
            Platform.Log(LogLevel.Info, "A copy of {0} has been saved to {1}.", _oldStudyInstanceUid, _backupDir);
        }

        private StudyXml LoadStudyXml(string studyXmlPath)
        {
            XmlDocument doc = new XmlDocument();
            StudyXml studyXml = new StudyXml();
			using (FileStream stream = FileStreamOpener.OpenForRead(studyXmlPath,FileMode.Open))
            {
                StudyXmlIo.Read(doc, stream);
                studyXml.SetMemento(doc);
				stream.Close();
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