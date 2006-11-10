using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// AdmissionType enumeration
    /// </summary>
	public enum AdmissionType
	{
        /// <summary>
        /// Unspecified
        /// </summary>
        [EnumValue("Unspecified")]
        X = 0,

        /// <summary>
        /// Accident
        /// </summary>
        [EnumValue("Accident")]
        A,

        /// <summary>
        /// Emergency
        /// </summary>
        [EnumValue("Emergency")]
        E,

        /// <summary>
        /// Labor and Delivery
        /// </summary>
        [EnumValue("Labor and Delivery")]
        L,

        /// <summary>
        /// Routine
        /// </summary>
        [EnumValue("Routine")]
        R,

        /// <summary>
        /// Newborn
        /// </summary>
        [EnumValue("Newborn")]
        N,

        /// <summary>
        /// Urgent
        /// </summary>
        [EnumValue("Urgent")]
        U,

        /// <summary>
        /// Elective
        /// </summary>
        [EnumValue("Elective")]
        C
    }
}