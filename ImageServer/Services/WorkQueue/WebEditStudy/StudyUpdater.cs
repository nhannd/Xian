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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Argument for events fired before a SOP is updated by the <see cref="StudyUpdater"/>
    /// </summary>
    internal class SopUpdatingEventArgs:EventArgs
    {
        #region Private Members
        private string _path;
        private string _uid;
        #endregion

        #region Public Properties

        /// <summary>
        /// Path of the Sop being updated
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// SOP Instance Uid of the SOP being updated.
        /// </summary>
        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        #endregion

    }

    /// <summary>
    /// Argument for events fired after a SOP has been updated by the <see cref="StudyUpdater"/>
    /// </summary>
    internal class SopUpdatedEventArgs : EventArgs
    {
        #region Private Members
        private string _path;
        private string _uid;
        #endregion

        #region Public Properties

        /// <summary>
        /// Path of the Sop that has been updated
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// SOP Instance Uid of the SOP being updated.
        /// </summary>
        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        #endregion

    }

    /// <summary>
    /// Argument for events fired after a SOP has been updated by the <see cref="StudyUpdater"/>
    /// </summary>
    internal class StudyUpdatedEventArgs : EventArgs
    {
        #region Private Members
        private string _newStudyInstanceUid;
        private string _oldStudyInstanceUid;
        private string _path;
        #endregion

        #region Public Properties

        /// <summary>
        /// The new study Instance Uid of the study.
        /// </summary>
        public string StudyInstanceUid
        {
            get { return _newStudyInstanceUid; }
            set { _newStudyInstanceUid = value; }
        }

        /// <summary>
        /// Storage location of the study
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// The study instance uid of the study before it was updated.
        /// </summary>
        public string OldStudyInstanceUid
        {
            get { return _oldStudyInstanceUid; }
            set { _oldStudyInstanceUid = value; }
        }

        #endregion

    }

    /// <summary>
    /// Provides study update functionality.
    /// </summary>
    /// <remarks>
    /// This class updates both the filesystems and the datbase but does not provide rollback operation if error occurs.
    /// It should be accomplished with different mean.
    /// </remarks>
    class StudyUpdater
    {
        #region Private Members
        private readonly StudyStorageLocation _studyStorage;
        private readonly string _workingDir = ServerPlatform.GetTempPath();
        private readonly ServerPartition _partition;
        private readonly string _oldStudyInstanceUid;
        private readonly string _newStudyPath;
        private readonly IList<IImageLevelUpdateCommand> _commands;
        private readonly string _oldStudyFolder;
        private readonly string _oldStudyPath;
        
        private Study _study;
        private Patient _patient;
        private StudyStorage _storage;
        private string _newStudyFolder;
        private string _newStudyInstanceUid;
        private EventHandler<SopUpdatingEventArgs> _sopUpdating;
        private EventHandler<SopUpdatedEventArgs> _sopUpdated;
        private EventHandler<StudyUpdatedEventArgs> _studyUpdated;


        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="StudyUpdater"/> to update a study.
        /// </summary>
        /// <param name="partition">The server partition where the study is stored</param>
        /// <param name="studyStorage"><see cref="StudyStorageLocation"/> record of the study</param>
        /// <param name="commands">Image level commands</param>
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
                    }
                }
                
            }

            Platform.CheckForNullReference(_newStudyInstanceUid, "_newStudyInstanceUid");

            _newStudyPath = Path.Combine(studyStorage.FilesystemPath, partition.PartitionFolder);
            _newStudyPath = Path.Combine(NewStudyPath, _newStudyFolder);
            _newStudyPath = Path.Combine(NewStudyPath, _newStudyInstanceUid);
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the study folder name of the study after it is updated.
        /// </summary>
        public string NewStudyFolder
        {
            get { return _newStudyFolder; }
        }

        /// <summary>
        /// Gets the study instance uid of the study after it is updated.
        /// </summary>
        public string NewStudyInstanceUid
        {
            get { return _newStudyInstanceUid; }
        }

        /// <summary>
        /// Gets the study path of the study after it is updated.
        /// </summary>
        public string NewStudyPath
        {
            get { return _newStudyPath; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event fired when a SOP is about to be updated.
        /// </summary>
        public event EventHandler<SopUpdatingEventArgs> SopUpdating
        {
            add { _sopUpdating+=value; }
            remove { _sopUpdating -= value; }
        }

        /// <summary>
        /// Event fired after a SOP has been updated.
        /// </summary>
        public event EventHandler<SopUpdatedEventArgs> SopUpdated
        {
            add { _sopUpdated += value; }
            remove { _sopUpdated -= value; }
        }

        /// <summary>
        /// Event fired after the study has been updated.
        /// </summary>
        public event EventHandler<StudyUpdatedEventArgs> StudyUpdated
        {
            add { _studyUpdated += value; }
            remove { _studyUpdated -= value; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the study using the specified <see cref="IUpdateContext"/> for database transaction.
        /// </summary>
        /// <param name="updateContext"></param>
        public void Update(IUpdateContext updateContext)
        {
            if (_commands == null || _commands.Count == 0)
            {
                Platform.Log(LogLevel.Info, "Nothing to update");
                LoadEntities();
            }
            else
            {

                PrintUpdateCommands();

                StageWorkingFolder();

                UpdateFilesystem();
                UpdateDatabase(updateContext);

                OnStudyUpdated(_oldStudyInstanceUid, NewStudyInstanceUid, NewStudyPath);
            }



        }
        
        #endregion


        #region Virtual Protected Methods
        protected virtual void OnStudyUpdated(string oldStudyInstanceUid, string studyInstanceUid, string path)
        {
            StudyUpdatedEventArgs arg = new StudyUpdatedEventArgs();
            arg.StudyInstanceUid = studyInstanceUid;
            arg.OldStudyInstanceUid = oldStudyInstanceUid;
            arg.Path = path;
            EventsHelper.Fire(_studyUpdated, this, arg);
        }

        protected virtual void OnUpdatingSop(string uid, string path)
        {
            SopUpdatingEventArgs arg = new SopUpdatingEventArgs();
            arg.Path = path;
            arg.Uid = uid;
            EventsHelper.Fire(_sopUpdating, this, arg);
        }

        protected virtual void OnSopUpdated(string uid, string path)
        {
            SopUpdatedEventArgs arg = new SopUpdatedEventArgs();
            arg.Path = path;
            arg.Uid = uid;
            EventsHelper.Fire(_sopUpdated, this, arg);
        }

        #endregion

        #region Private Members

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

        private void UpdateFilesystem()
        {
            Platform.Log(LogLevel.Debug, "Updating study filesystem...");
            
            StudyXml studyXml = new StudyXml(_newStudyInstanceUid);
            FileProcessor.Process(_workingDir, "*.dcm",
                                  delegate(string path)
                                      {
                                          // this should be the updated study instance
                                          DicomFile file = new DicomFile(path);
                                          file.Load(DicomReadOptions.StorePixelDataReferences);

                                          OnUpdatingSop(file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty), path);
                                          SaveFile(file);
                                          studyXml.AddFile(file);

                                      }, true);

            Platform.Log(LogLevel.Debug, "Generating study header...");
            string studyXmlPath = Path.Combine(NewStudyPath, _newStudyInstanceUid + ".xml");
            string gzipStudyXmlPath = Path.Combine(NewStudyPath, _newStudyInstanceUid + ".xml.gz");
            using (FileStream xmlStream = new FileStream(studyXmlPath, FileMode.Create),
                              gzipStream = new FileStream(gzipStudyXmlPath, FileMode.Create))
            {
                StudyXmlIo.WriteXmlAndGzip(studyXml.GetMemento(new StudyXmlOutputSettings()), xmlStream, gzipStream);
            }

            if (NewStudyPath != _oldStudyPath)
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

            OnSopUpdated(sopInstanceUid, destPath);
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
            _storage = StudyStorage.Load(_studyStorage.GetKey());
            _study = Study.Find(_storage.StudyInstanceUid, _partition);
            _patient = Patient.Load(_study.PatientKey);
        }


        private void StageWorkingFolder()
        {
            Platform.Log(LogLevel.Debug, "Setting up working folder {0}...", _workingDir);
            string studyPath = _studyStorage.GetStudyPath();
            Directory.CreateDirectory(_workingDir);
            StudyXml studyXml = new StudyXml();
            XmlDocument doc = new XmlDocument();
            string studyXmlPath = Path.Combine(studyPath, _studyStorage.StudyInstanceUid + ".xml");

            using(FileStream stream = File.OpenRead(studyXmlPath))
            {
                StudyXmlIo.Read(doc, stream);
                studyXml.SetMemento(doc);    
            }
            
            Platform.Log(LogLevel.Debug, "Copying files into working folder...");
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


            Platform.Log(LogLevel.Debug, "Updating files in working folder..."); 
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
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_workingDir != null)
            {
                DirectoryUtility.DeleteIfExists(_workingDir);
            }
        }

        #endregion
    }
}