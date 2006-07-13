using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Simplified implementation of HL7 CX (Extended Composite ID) data type
    /// </summary>
    public class PatientIdentifier
    {
        private string _id;
        private string _assigningAuthority;
        private PatientIdentifierType _type;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string AssigningAuthority
        {
            get { return _assigningAuthority; }
            set { _assigningAuthority = value; }
        }

        public PatientIdentifierType Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}
