using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model
{
    public class StudyStorageLocation : ServerEntity
    {
        #region Constructors
        public StudyStorageLocation()
            : base("StudyStorageLocation")
        {
        }
        #endregion

        #region Private Members
        private ServerEntityKey _serverPartitionKey;
        private ServerEntityKey _filesystemKey;
        private string _studyInstanceUid;
        private DateTime _lastAccessed;
        private short _statusEnum;
        private String _filesystemPath;
        private String _serverPartitionFolder;
        private String _storageFilesystemFolder;
        private bool _enabled;
        private bool _readOnly;
        private bool _writeOnly;
        #endregion

        #region Public Properties
        public ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }
        public ServerEntityKey FilesystemKey
        {
            get { return _filesystemKey; }
            set { _filesystemKey = value; }
        }        
        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }
        public DateTime LastAccessedTime
        {
            get { return _lastAccessed; }
            set { _lastAccessed = value; }
        }
        public short StatusEnum
        {
            get { return _statusEnum; }
            set { _statusEnum = value; }
        }
        public string FilesystemPath
        {
            get { return _filesystemPath; }
            set { _filesystemPath = value; }
        }
        public string PartitionFolder
        {
            get { return _serverPartitionFolder; }
            set { _serverPartitionFolder = value; }
        }
        public string StudyFolder
        {
            get { return _storageFilesystemFolder; }
            set { _storageFilesystemFolder = value; }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }
        public bool WriteOnly
        {
            get { return _writeOnly; }
            set { _writeOnly = value; }
        }
        #endregion

        #region Public Methods
        public string GetStudyPath()
        {
            string path = Path.Combine(FilesystemPath, this.PartitionFolder);
            path = Path.Combine(path, this.StudyFolder);
            path = Path.Combine(path, this.StudyInstanceUid);

            return path;
        }
        #endregion
    }
}
