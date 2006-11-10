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
        PRE,

        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending")]
        PND,

        /// <summary>
        /// Active/Admitted
        /// </summary>
        [EnumValue("Active/Admitted")]
        A,

        /// <summary>
        /// Discharged
        /// </summary>
        [EnumValue("Discharged")]
        D
    }
}