using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// NoteSeverity enumeration
    /// </summary>
	public enum NoteSeverity
	{
        /// <summary>
        /// Low
        /// </summary>
        [EnumValue("Low")]
        L,

        /// <summary>
        /// Normal
        /// </summary>
        [EnumValue("Normal")]
        N,

        /// <summary>
        /// High
        /// </summary>
        [EnumValue("High")]
        H
    }
}