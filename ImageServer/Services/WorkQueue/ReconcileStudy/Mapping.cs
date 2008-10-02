using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Mapping for study information used for study reconciliation
    /// </summary>
    class StudyInfoMapping
    {
        #region Private Members
        private string _studyId;
        private string _studyInstanceUid;
        private string _studyDescription;
        private string _studyDate;
        private string _studyTime;
        private string _accessionNumber;
        #endregion

        #region Public Properties
        [DicomField(DicomTags.StudyId)]
        public string StudyId
        {
            get { return _studyId; }
            set { _studyId = value; }
        }

        [DicomField(DicomTags.StudyInstanceUid)]
        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        [DicomField(DicomTags.StudyDescription)]
        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        [DicomField(DicomTags.StudyDate)]
        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        [DicomField(DicomTags.StudyTime)]
        public string StudyTime
        {
            get { return _studyTime; }
            set { _studyTime = value; }
        }

        [DicomField(DicomTags.AccessionNumber)]
        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        #endregion

    }

    /// <summary>
    /// Mapping for patient information used for study reconciliation
    /// </summary>
    class DemographicInfo
    {
        #region Private Members
        private string _patientsName;
        private string _patientId;
        private string _IssuerOfPatientId;
        private string _patientsBirthdate;
        private string _patientsSex;
        #endregion

        #region Public Properties
        [DicomField(DicomTags.PatientsName)]
        public string PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

        [DicomField(DicomTags.PatientId)]
        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        [DicomField(DicomTags.IssuerOfPatientId)]
        public string IssuerOfPatientId
        {
            get { return _IssuerOfPatientId; }
            set { _IssuerOfPatientId = value; }
        }

        [DicomField(DicomTags.PatientsBirthDate)]
        public string PatientsBirthDate
        {
            get { return _patientsBirthdate; }
            set { _patientsBirthdate = value; }
        }

        [DicomField(DicomTags.PatientsSex)]
        public string PatientsSex
        {
            get { return _patientsSex; }
            set { _patientsSex = value; }
        }

        #endregion

    }
}