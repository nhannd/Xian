using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// VisitStatus enumeration
    /// </summary>
    [EnumValueClass(typeof(VisitStatusEnum))]
    public enum VisitStatus
	{
        /// <summary>
        /// Pre-Admit
        /// </summary>
        [EnumValue("Pre-Admit")]
        PA,

        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending")]
        PD,

        /// <summary>
        /// Active/Admitted
        /// </summary>
        [EnumValue("Active/Admitted")]
        AA,

        /// <summary>
        /// Discharged
        /// </summary>
        [EnumValue("Discharged")]
        DC,

        /// <summary>
        /// Cancelled
        /// </summary>
        [EnumValue("Cancelled")]
        CX,

        /// <summary>
        /// Pre-Admit - Cancelled
        /// </summary>
        [EnumValue("Pre-Admit - Cancelled")]
        PC
    }
}