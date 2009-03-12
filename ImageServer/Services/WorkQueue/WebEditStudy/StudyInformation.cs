using System;
using System.Xml.Serialization;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Represents the information of a study.
    /// </summary>
    [XmlRoot(Namespace = "http://www.clearcanvas.ca/ImageServer/WebEditStudy/StudyInformation")]
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

    /// <summary>
    /// Represents the information of a patient that is relevant to the study edit operation.
    /// </summary>
    public class PatientInformation
    {
        #region Private Fields
        private string _name;
        private string _patientId;
        private string _issuerOfPatientId;
        private DateTime? _birthdate;
        private string _age;
        private string _sex;
        #endregion

        #region Public Properties
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

        [XmlElement(DataType = "date")]
        public DateTime? Birthdate
        {
            get { return _birthdate; }
            set { _birthdate = value; }
        }

        public string Age
        {
            get { return _age; }
            set { _age = value; }
        }

        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        public string IssuerOfPatientId
        {
            get { return _issuerOfPatientId; }
            set { _issuerOfPatientId = value; }
        }

        #endregion
    }
}