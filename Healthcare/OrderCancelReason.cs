using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// OrderCancelReason enumeration
    /// </summary>
	public enum OrderCancelReason
	{
        /// <summary>
        /// Cancelled by patient
        /// </summary>
        [EnumValue("Cancelled by patient")]
        PA,

        /// <summary>
        /// Cancelled by Physician
        /// </summary>
        [EnumValue("Cancelled by Physician")]
        PH,

        /// <summary>
        /// Patient is claustrophobic
        /// </summary>
        [EnumValue("Patient is claustrophobic")]
        CL,

        /// <summary>
        /// Patient has an allergy
        /// </summary>
        [EnumValue("Patient has an allergy")]
        AL,

        /// <summary>
        /// Patient did not show for exam    
        /// </summary>
        [EnumValue("Patient did not show for exam")]
        NS
    }
}