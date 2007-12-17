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
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    public class FilesystemQueue : ServerEntity
    {
        #region Constructors
        public FilesystemQueue()
            : base("FilesystemQueue")
        {
        }
        #endregion

        #region Private Members
        private FilesystemQueueTypeEnum _fileSystemQueueTypeEnum;
        private ServerEntityKey _studyStorageKey;
        private ServerEntityKey _filesystemKey;
        private DateTime _scheduledTime;
        private String _seriesInstanceUid;
        #endregion

        #region Public Properties
        public FilesystemQueueTypeEnum FilesystemQueueTypeEnum
        {
            get { return _fileSystemQueueTypeEnum; }
            set { _fileSystemQueueTypeEnum = value; }
        }
        public ServerEntityKey StudyStorageKey
        {
            get { return _studyStorageKey; }
            set { _studyStorageKey = value; }
        }
        public ServerEntityKey FilesystemKey
        {
            get { return _filesystemKey; }
            set { _filesystemKey = value; }
        }
        public DateTime ScheduledTime
        {
            get { return _scheduledTime; }
            set { _scheduledTime = value; }
        }
        public String SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }
        #endregion
    }
}
