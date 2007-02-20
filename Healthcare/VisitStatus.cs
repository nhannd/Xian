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
        Preadmitted,

        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending")]
        Pending,

        /// <summary>
        /// Active/Admitted
        /// </summary>
        [EnumValue("Active/Admitted")]
        Admitted,

        /// <summary>
        /// Discharged
        /// </summary>
        [EnumValue("Discharged")]
        Discharged,

        /// <summary>
        /// Discharged
        /// </summary>
        [EnumValue("Cancelled")]
        Cancelled,

        /// <summary>
        /// Discharged
        /// </summary>
        [EnumValue("Pre-Admit - Cancelled")]
        PreadmitCancelled
    }
}