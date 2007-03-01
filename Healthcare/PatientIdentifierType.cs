using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// PatientIdentifierType enumeration - see HL7 2.9.12.5
    /// </summary>
	public enum PatientIdentifierType
	{
        /// <summary>
        /// MRN - Medical Record Number
        /// </summary>
        [EnumValue("MRN", Description="Medical Record Number")]
        MR,

        /// <summary>
        /// Healthcard Number
        /// </summary>
        [EnumValue("Healthcard")]
        HC,
	}
}