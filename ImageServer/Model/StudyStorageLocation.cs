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
using System.IO;
using ClearCanvas.ImageServer.Enterprise;

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
        private DateTime _insertTime;
        private StudyStatusEnum _statusEnum;
        private String _filesystemPath;
        private String _serverPartitionFolder;
        private String _storageFilesystemFolder;
        private bool _lock; 
        private bool _enabled;
        private bool _readOnly;
        private bool _writeOnly;
        private FilesystemTierEnum _filesystemTierEnum;
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
        public DateTime InsertTime
        {
            get { return _insertTime; }
            set { _insertTime = value; }
        }
        public bool Lock
        {
            get { return _lock; }
            set { _lock = value; }
        }
        public StudyStatusEnum StudyStatusEnum
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
        public FilesystemTierEnum FilesystemTierEnum
        {
            get { return _filesystemTierEnum; }
            set { _filesystemTierEnum = value; }
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
