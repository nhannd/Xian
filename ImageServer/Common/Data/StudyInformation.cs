using System;
using System.Xml.Serialization;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common.Data
{
    /// <summary>
    /// Represents the information of a study.
    /// </summary>
    [XmlRoot("StudyInformation")]
    public class StudyInformation
    {
        #region Private Fields
        private string _studyId;
        private string _accessionNumber;
        private DateTime? _studyDateTime;
        private string _modalities;
        private string _studyInstanceUid;
        private string _studyDescription;
        private string _referringPhysician;
        private PatientInformation _patientInfo = new PatientInformation();
        #endregion

        #region Public Properties
        public string StudyId
        {
            get { return _studyId; }
            set { _studyId = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public DateTime? StudyDateTime
        {
            get { return _studyDateTime; }
            set { _studyDateTime = value; }
        }

        public string Modalities
        {
            get { return _modalities; }
            set { _modalities = value; }
        }

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public string ReferringPhysician
        {
            get { return _referringPhysician; }
            set { _referringPhysician = value; }
        }

        public PatientInformation PatientInfo
        {
            get { return _patientInfo; }
            set { _patientInfo = value; }
        }

        #endregion

        #region Public Static Methods
        public static StudyInformation CreateFrom(Study study)
        {
            StudyInformation studyInfo = new StudyInformation();
            studyInfo.AccessionNumber = study.AccessionNumber;
            studyInfo.ReferringPhysician = study.ReferringPhysiciansName;

            DateTime dt;
            if (DateTimeParser.ParseDateAndTime(String.Empty, study.StudyDate, study.StudyTime, out dt))
                studyInfo.StudyDateTime = dt;

            studyInfo.StudyDescription = study.StudyDescription;
            studyInfo.StudyId = study.StudyId;
            studyInfo.StudyInstanceUid = study.StudyInstanceUid;

            studyInfo.PatientInfo = new PatientInformation();
            studyInfo.PatientInfo.Age = study.PatientsAge;
            if (DateParser.Parse(study.PatientsBirthDate, out dt))
                studyInfo.PatientInfo.Birthdate = dt;

            studyInfo.PatientInfo.Name = study.PatientsName;
            studyInfo.PatientInfo.PatientId = study.PatientId;
            studyInfo.PatientInfo.IssuerOfPatientId = study.IssuerOfPatientId;


            return studyInfo;
        }
        #endregion
    }
}