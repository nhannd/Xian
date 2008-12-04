using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Web.Common.Data.Model
{
    public class DeletedStudyInfo
    {
        private object _key;
        private string _studyInstanceUid;
        private string _patientsName;
        private string _patientId;
        private string _accessionNumber;
        private string _studyDate;
        private string _partitionAE;
        private string _studyDescription;
        private string _deletedFolderPath;
        private string _reasonForDeletion;
        private DateTime _deleteTime;
        private ServerEntityKey _archiveLocation;

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
            get { return _partitionAE; }
            set { _partitionAE = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public object Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public string DeletedFolderPath
        {
            get { return _deletedFolderPath; }
            set { _deletedFolderPath = value; }
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

        public ServerEntityKey ArchiveLocation
        {
            get { return _archiveLocation; }
            set { _archiveLocation = value; }
        }
    }
}
