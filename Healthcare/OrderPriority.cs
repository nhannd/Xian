using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// OrderPriority enumeration, defined by HL7 (see Timing Quantity TQ data-type)
    /// </summary>
	public enum OrderPriority
	{
        /// <summary>
        /// Routine
        /// </summary>
        [EnumValue("Routine")]
        R,

        /// <summary>
        /// Urgent (or ASAP as defined by HL7)
        /// </summary>
        [EnumValue("Urgent")]
        A,

        /// <summary>
        /// Stat
        /// </summary>
        [EnumValue("Stat")]
        S
	}
}