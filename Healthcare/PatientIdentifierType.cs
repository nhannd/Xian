using System;
using System.Collections;
using System.Text;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// PatientIdentifierType enumeration - see HL7 2.9.12.5
    /// </summary>
	public enum PatientIdentifierType
	{
        /// <summary>
        /// MRN - Medical Record Number
        /// </summary>
        MR,

        /// <summary>
        /// Healthcard Number
        /// </summary>
        HC,
	}
}