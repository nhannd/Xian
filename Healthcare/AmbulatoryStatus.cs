using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// AmbulatoryStatus enumeration
    /// </summary>
	public enum AmbulatoryStatus
	{
        /// <summary>
        /// Unspecified
        /// </summary>
        [EnumValue("Unspecified")]
        X = 0,

        /// <summary>
        /// No functional limitations
        /// </summary>
        [EnumValue("No Functional limitations")]
        A0,

        /// <summary>
        /// Ambulates with assistive device
        /// </summary>
        [EnumValue("Ambulates with assistive device")]
        A1,

        /// <summary>
        /// Wheelchair/stretcher bound
        /// </summary>
        [EnumValue("Wheelchair/stretcher bound")]
        A2,

        /// <summary>
        /// No functional limitations
        /// </summary>
        [EnumValue("Comatose; non-responsive")]
        A3

        ///etc.
    }
}