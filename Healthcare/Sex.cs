using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// HL7 Sex enumeration
    /// </summary>
	public enum Sex
	{
        /// <summary>
        /// Unknown
        /// </summary>
        [EnumValue("Unknown")]
        U,

        /// <summary>
        /// Female
        /// </summary>
        [EnumValue("Female")]
        F,

        /// <summary>
        /// Male
        /// </summary>
        [EnumValue("Male")]
        M,

        /// <summary>
        /// Other
        /// </summary>
        [EnumValue("Other")]
        O,

        /// <summary>
        /// Ambiguous
        /// </summary>
        [EnumValue("Ambiguous")]
        A,

        /// <summary>
        /// Not applicable
        /// </summary>
        [EnumValue("Not Applicable")]
        N
	}
}