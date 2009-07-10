using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Core.Edit
{
    /// <summary>
    /// Contains a list of fields that will be updated when processing duplicates. This is the list
    /// in the server partition configuration.
    /// </summary>
    public class StudyMatchingMap
    {
        #region Private Members
        private string _patientsName;
        private string _patientId;
        private string _IssuerOfPatientId;
        private string _patientsBirthdate; 
        private string _patientsSex;
        private string _accessionNumber;
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

        [DicomField(DicomTags.AccessionNumber)]
        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        #endregion

    }
}
