using System;
using System.Xml.Serialization;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Common.Data
{
    /// <summary>
    /// Represents the serializable patient information
    /// </summary>
    [XmlRoot("Patient")]
    public class PatientInformation
    {
        #region Private Fields
        private string _name;
        private string _patientId;
        private string _issuerOfPatientId;
        private string _birthdate;
        private string _age;
        private string _sex;
        #endregion

        #region Constructors
        public PatientInformation()
        {
        }

        public PatientInformation(IDicomAttributeProvider attributeProvider)
        {
            if (attributeProvider[DicomTags.PatientsName] != null)
                Name = attributeProvider[DicomTags.PatientsName].ToString();

            if (attributeProvider[DicomTags.PatientId] != null)
                PatientId = attributeProvider[DicomTags.PatientId].ToString();

            if (attributeProvider[DicomTags.PatientsAge] != null)
                Age = attributeProvider[DicomTags.PatientsAge].ToString();

            if (attributeProvider[DicomTags.PatientsBirthDate] != null)
                PatientsBirthdate = attributeProvider[DicomTags.PatientsBirthDate].ToString();

            if (attributeProvider[DicomTags.PatientsSex] != null)
                Sex = attributeProvider[DicomTags.PatientsSex].ToString();

            if (attributeProvider[DicomTags.IssuerOfPatientId] != null)
                IssuerOfPatientId = attributeProvider[DicomTags.IssuerOfPatientId].ToString();
        }
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


        public string PatientsBirthdate
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