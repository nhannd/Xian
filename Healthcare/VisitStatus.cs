using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// VisitStatus enumeration
    /// </summary>
	public enum VisitStatus
	{
        /// <summary>
        /// Pre-Admit
        /// </summary>
        [EnumValue("Pre-Admit")]
        PREADMITTED,

        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending")]
        PENDING,

        /// <summary>
        /// Active/Admitted
        /// </summary>
        [EnumValue("Active/Admitted")]
        ADMITTED,

        /// <summary>
        /// Discharged
        /// </summary>
        [EnumValue("Discharged")]
        DISCHARGED,

        /// <summary>
        /// Discharged
        /// </summary>
        [EnumValue("Cancelled")]
        CANCELLED,

        /// <summary>
        /// Discharged
        /// </summary>
        [EnumValue("Pre-Admit - Cancelled")]
        PREADMITCANCELLED
    }
}