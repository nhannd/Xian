#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;

namespace ClearCanvas.ImageServer.Web.Common.Data.Model
{
    [Serializable]
    public class DeletedStudyInfo
    {
        #region Private Fields
        private object _rowkey;
        private string _studyInstanceUid;
        private string _patientsName;
        private string _patientId;
        private string _accessionNumber;
        private string _studyDate;
        private string _partitionAe;
        private string _studyDescription;
        private string _backupFolderPath;
        private string _reasonForDeletion;
        private DateTime _deleteTime;
        private ServerEntityKey _deleteStudyRecord;
        private  DeletedStudyArchiveInfoCollection _archives;

        private bool _restoreScheduled;
        private string _userName;
        private string _userId;

        #endregion

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public string PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        public string PartitionAE
        {
            get { return _partitionAe; }
            set { _partitionAe = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public object RowKey
        {
            get { return _rowkey; }
            set { _rowkey = value; }
        }

        public string BackupFolderPath
        {
            get { return _backupFolderPath; }
            set { _backupFolderPath = value; }
        }

        public string ReasonForDeletion
        {
            get { return _reasonForDeletion; }
            set { _reasonForDeletion = value; }
        }

        public DateTime DeleteTime
        {
            get { return _deleteTime; }
            set { _deleteTime = value; }
        }

        public DeletedStudyArchiveInfoCollection Archives
        {
            get { return _archives; }
            set { _archives = value; }
        }

        public bool RestoreScheduled
        {
            get { return _restoreScheduled; }
            set { _restoreScheduled = value; }
        }

        public ServerEntityKey DeleteStudyRecord
        {
            get { return _deleteStudyRecord; }
            set { _deleteStudyRecord = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
    }

}
