using System;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    class PatientInfo : IEquatable<PatientInfo>
    {
        private string _name;
        private string _patientId;
        private string _issuerOfPatientId;

        public PatientInfo()
        {
        }

        public PatientInfo(PatientInfo other)
        {
            Name = other.Name;
            PatientId = other.PatientId;
            IssuerOfPatientId = other.IssuerOfPatientId;
        }
        
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

        public string IssuerOfPatientId
        {
            get { return _issuerOfPatientId; }
            set { _issuerOfPatientId = value; }
        }

        #region IEquatable<PatientInfo> Members

        public bool Equals(PatientInfo other)
        {
            PersonName name = new PersonName(_name);
            PersonName otherName = new PersonName(other.Name);
            return name.Equals(otherName) && String.Equals(_patientId, other.PatientId, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}