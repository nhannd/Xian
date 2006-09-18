using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// VisitLocationRole enumeration
    /// </summary>
	public enum VisitLocationRole
	{
        /// <summary>
        /// Current
        /// </summary>
        [EnumValue("Current")]
        CR,

        /// <summary>
        /// Temporary
        /// </summary>
        [EnumValue("Temporary")]
        TM,

        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending")]
        PN,

        /// <summary>
        /// Prior
        /// </summary>
        [EnumValue("Prior")]
        PR
	}
}