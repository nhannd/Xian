using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Patient entity
    /// </summary>
	public partial class Patient : Entity
	{
		/// <summary>
		/// Factory method
		/// </summary>
		public static Patient New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
			Patient patient = new Patient();
            patient._sex = Sex.U;   // default to Unknown
            return patient;
		}

        /// <summary>
        /// Returns the Mrn, assuming one exists, or null if none exists.
        /// This method will need to change in future to deal with multiple Mrns. 
        /// </summary>
        /// <returns></returns>
        public PatientIdentifier GetMrn()
        {
            foreach(PatientIdentifier identifier in _identifiers)
            {
                if(identifier.Type == PatientIdentifierType.MR)
                    return identifier;
            }
            return null;
        }
	}
}