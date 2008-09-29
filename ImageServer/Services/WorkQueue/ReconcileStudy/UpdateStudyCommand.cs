using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    class UpdateStudyCommand :ServerDatabaseCommand
    {
        readonly StudyUpdater _updater;
        
        public UpdateStudyCommand(ServerPartition partition, 
                                  StudyStorageLocation studyLcation,
                                  IList<IImageLevelUpdateCommand> _imageLevelCommands) 
            : base("Update existing study", true)
        {
            _updater = new StudyUpdater(partition, studyLcation, _imageLevelCommands);
        }

        public String NewStudyPath
        {
            get
            {
                return _updater.NewStudyPath;
            }
        }

        public String NewStudyInstanceUid
        {
            get
            {
                return _updater.NewStudyInstanceUid;
            }
        }

        public String NewStudyFolder
        {
            get
            {
                return _updater.NewStudyFolder;
            }
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            _updater.Update(updateContext);
        }

        protected override void OnUndo()
        {
            _updater.Rollback();
        }
    }

    class StudyUpdater
    {
        private readonly StudyStorageLocation _studyStorage;
        private readonly string _workingDir = Path.Combine(Path.GetPathRoot(Path.GetTempPath()), Path.GetRandomFileName());
        private readonly string _backupDir = Path.Combine(Path.GetPathRoot(Path.GetTempPath()), Path.GetRandomFileName());

        private readonly ServerPartition _partition;
        private Study _study;
        private Patient _patient;
        private StudyStorage _storage;
        private string _newStudyFolder;
        private readonly string _oldStudyFolder;
        private readonly string _oldStudyPath;
        private string _newStudyInstanceUid;
        private readonly string _oldStudyInstanceUid;
        private readonly string _newStudyPath;
        private readonly IList<IImageLevelUpdateCommand> _commands;

        public StudyUpdater(ServerPartition partition,
                            StudyStorageLocation studyStorage,
                            IList<IImageLevelUpdateCommand> commands)
        {
            _partition = partition;
            _commands = commands;
            _studyStorage = studyStorage;
            _oldStudyFolder = studyStorage.StudyFolder;
            _oldStudyPath = studyStorage.GetStudyPath();
            _oldStudyInstanceUid = studyStorage.StudyInstanceUid;
            _newStudyFolder = _oldStudyFolder;
            _newStudyInstanceUid = _oldStudyInstanceUid;

            foreach (IImageLevelUpdateCommand command in commands)
            {
                ImageLevelUpdateEntry imageLevelUpdate = command.UpdateEntry;
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
                }
            }

            Platform.CheckForNullReference(_newStudyInstanceUid, "_newStudyInstanceUid");

            _newStudyPath = Path.Combine(studyStorage.FilesystemPath, partition.PartitionFolder);
            _newStudyPath = Path.Combine(NewStudyPath, _newStudyFolder);
            _newStudyPath = Path.Combine(NewStudyPath, _newStudyInstanceUid);
        }

        public string NewStudyFolder
        {
            get { return _newStudyFolder; }
        }

        public string NewStudyInstanceUid
        {
            get { return _newStudyInstanceUid; }
        }

        public string NewStudyPath
        {
            get { return _newStudyPath; }
        }


        string GetNewStudyPath()
        {
            string path = Path.Combine(_studyStorage.FilesystemPath, _partition.PartitionFolder);
            path = Path.Combine(path, NewStudyFolder);
            path = Path.Combine(path, NewStudyInstanceUid);
            return path;
        }

        public void Update(IUpdateContext updateContext)
        {
            if (_commands == null || _commands.Count == 0)
            {
                Platform.Log(LogLevel.Info, "Nothing to update");
                LoadEntities();
            }
            else
            {
                BackupFilesystem();

                PrintUpdateCommands();

                StageWorkingFolder();


                FileProcessor.Process(_workingDir, "*.dcm",
                                      delegate(string path)
                                          {
                                              DicomFile file = new DicomFile(path);
                                              file.Load();

                                              foreach (IImageLevelUpdateCommand command in _commands)
                                              {
                                                  command.Apply(file);
                                              }
                                              file.Save();
                                          }, true);


                UpdateDatabase(updateContext);
                UpdateFilesystem();
            }



        }

        private void PrintUpdateCommands()
        {
            StringBuilder log = new StringBuilder();
            log.AppendFormat("Update Study with following changes:\n");
            foreach (IImageLevelUpdateCommand cmd in _commands)
            {
                log.AppendFormat("{0}", cmd);
                log.AppendLine();
            }

            Platform.Log(LogLevel.Info, log);
        }

        public void Rollback()
        {
            if (NewStudyFolder != _oldStudyFolder)
            {
                DirectoryUtility.Copy(_backupDir, _oldStudyFolder);
            }
            else
            {
                DirectoryUtility.DeleteIfExists(GetNewStudyPath());
            }

        }

        private void UpdateFilesystem()
        {
            StudyXml studyXml = new StudyXml(_newStudyInstanceUid);
            FileProcessor.Process(_workingDir, "*.dcm",
                                  delegate(string path)
                                      {
                                          // this should be the updated study instance
                                          DicomFile file = new DicomFile(path);
                                          file.Load(DicomReadOptions.StorePixelDataReferences);

                                          SaveFile(file);

                                          studyXml.AddFile(file);

                                      }, true);

            string studyXmlPath = Path.Combine(NewStudyPath, _newStudyInstanceUid + ".xml");
            string gzipStudyXmlPath = Path.Combine(NewStudyPath, _newStudyInstanceUid + ".xml.gz");
            using (FileStream xmlStream = new FileStream(studyXmlPath, FileMode.Create),
                              gzipStream = new FileStream(gzipStudyXmlPath, FileMode.Create))
            {
                StudyXmlIo.WriteXmlAndGzip(studyXml.GetMemento(new StudyXmlOutputSettings()), xmlStream, gzipStream);
            }

            if (NewStudyFolder != _oldStudyFolder)
            {
                Platform.Log(LogLevel.Info, "Removing existing folder... {0}", _oldStudyPath);

                DirectoryUtility.DeleteIfExists(_oldStudyPath, true);
            }

        }

        private void SaveFile(DicomFile file)
        {
            String seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            String sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);


            String destPath = _studyStorage.FilesystemPath;
            String extension = ".dcm";

            ServerCommandProcessor filesystemUpdateProcessor = new ServerCommandProcessor("Filesystem update processor");
            filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, _partition.PartitionFolder);
            filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, NewStudyFolder);
            filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

            destPath = Path.Combine(destPath, NewStudyInstanceUid);
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
                throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", file.Filename, filesystemUpdateProcessor.FailureReason));
            }
        }


        private void UpdateDatabase(IUpdateContext updateContext)
        {
            LoadEntities();

            UpdateEntity(_study);
            UpdateEntity(_patient);
            UpdateEntity(_storage);

            if (!String.IsNullOrEmpty(_study.StudyDate))
                _newStudyFolder = _study.StudyDate;

            _newStudyInstanceUid = _study.StudyInstanceUid;

            IStudyEntityBroker studyUpdateBroker = updateContext.GetBroker<IStudyEntityBroker>();
            studyUpdateBroker.Update(_study);

            IPatientEntityBroker patientUpdateBroker = updateContext.GetBroker<IPatientEntityBroker>();
            patientUpdateBroker.Update(_patient);

            IStudyStorageEntityBroker storageUpdateBroker = updateContext.GetBroker<IStudyStorageEntityBroker>();
            storageUpdateBroker.Update(_storage);


            IFilesystemStudyStorageEntityBroker filesystemStorageBroker = updateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
            FilesystemStudyStorageSelectCriteria criteria = new FilesystemStudyStorageSelectCriteria();
            criteria.FilesystemKey.Equals(_studyStorage.FilesystemKey);
            criteria.StudyStorageKey.EqualTo(_studyStorage.GetKey());
            FilesystemStudyStorageUpdateColumns columns = new FilesystemStudyStorageUpdateColumns();
            columns.StudyFolder = NewStudyFolder;
            filesystemStorageBroker.Update(criteria, columns);
        }

        private void UpdateEntity(ServerEntity entity)
        {
            EntityDicomMap entityMap = EntityDicomMapManager.Get(entity.GetType());

            foreach (IImageLevelUpdateCommand command in _commands)
            {
                ImageLevelUpdateEntry entry = command.UpdateEntry;
                if (entityMap.ContainsKey(entry.Tag))
                {
                    entityMap[entry.Tag].SetValue(entity, entry.GetStringValue(), null);
                }
            }

        }


        void LoadEntities()
        {
            _storage = StudyStorage.Load(_studyStorage.GetKey());
            _study = FindStudy(_storage.StudyInstanceUid, _partition);
            _patient = Patient.Load(_study.PatientKey);
        }

        protected Study FindStudy(string studyInstanceUid, ServerPartition partition)
        {
            IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
            IStudyEntityBroker broker = ctx.GetBroker<IStudyEntityBroker>();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(partition.GetKey());
            criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
            _study = broker.FindOne(criteria);

            return _study;
        }


        private void StageWorkingFolder()
        {
            string studyPath = _studyStorage.GetStudyPath();
            Directory.CreateDirectory(_workingDir);

            StudyXml studyXml = new StudyXml();
            XmlDocument doc = new XmlDocument();
            string studyXmlPath = Path.Combine(studyPath, _studyStorage.StudyInstanceUid + ".xml");

            FileStream stream = File.OpenRead(studyXmlPath);
            StudyXmlIo.Read(doc, stream);
            studyXml.SetMemento(doc);

            foreach (SeriesXml seriesXml in studyXml)
            {
                foreach (InstanceXml instanceXml in seriesXml)
                {
                    string path = Path.Combine(studyPath, seriesXml.SeriesInstanceUid);
                    path = Path.Combine(path, instanceXml.SopInstanceUid);
                    path += ".dcm";

                    File.Copy(path, Path.Combine(_workingDir, instanceXml.SopInstanceUid) + ".dcm");

                }
            }
        }

        private void BackupFilesystem()
        {
            DirectoryUtility.Copy(_oldStudyPath, _backupDir);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_workingDir != null)
            {
                DirectoryUtility.DeleteIfExists(_workingDir);
            }
            if (_backupDir != null)
                DirectoryUtility.DeleteIfExists(_backupDir);
        }

        #endregion
    }
}