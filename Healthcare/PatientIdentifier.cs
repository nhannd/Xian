using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Simplified implementation of HL7 CX (Extended Composite ID) data type
    /// </summary>
	public partial class PatientIdentifier
	{
		/// <summary>
		/// Factory method
		/// </summary>
		public static PatientIdentifier New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
            PatientIdentifier patientIdentifier = new PatientIdentifier();
            patientIdentifier.Type = PatientIdentifierType.MR;
            patientIdentifier.AssigningAuthority = "UHN";
            return patientIdentifier;
		}

        public void CopyFrom(PatientIdentifier source)
        {
            _id = source.Id;
            _assigningAuthority = source.AssigningAuthority;
            _type = source.Type;
        }
	}
}