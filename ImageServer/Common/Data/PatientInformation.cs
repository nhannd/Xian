using System;
using System.Xml.Serialization;

namespace ClearCanvas.ImageServer.Common.Data
{
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