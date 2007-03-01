using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// PatientClass enumeration
    /// </summary>
	public enum PatientClass
	{
        /// <summary>
        /// Emergency
        /// </summary>
        [EnumValue("Emergency")]
        E,

        /// <summary>
        /// Inpatient
        /// </summary>
        [EnumValue("Inpatient")]
        I,

        /// <summary>
        /// Outpatient
        /// </summary>
        [EnumValue("Outpatient")]
        O,

        /// <summary>
        /// Preadmit
        /// </summary>
        [EnumValue("Preadmit")]
        P,

        /// <summary>
        /// Recurring
        /// </summary>
        [EnumValue("Recurring")]
        R,

        /// <summary>
        /// Obstetrics
        /// </summary>
        [EnumValue("Obstetrics")]
        B,

        /// <summary>
        /// Not applicable
        /// </summary>
        [EnumValue("Not applicable")]
        N,

        /// <summary>
        /// Unknown
        /// </summary>
        [EnumValue("Unknown")]
        U
	}
}