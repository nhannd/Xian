using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Data.Model
{
    public class DeletedStudyInfo
    {
        private string _studyInstanceUid;
        private string _patientsName;
        private string _patientId;
        private string _accessionNumber;
        private string _studyDate;
        private string _partitionAE;
        private string _studyDescription;

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
    }
}
