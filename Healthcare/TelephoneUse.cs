using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// TelephoneUse enumeration
    /// </summary>
	public enum TelephoneUse
	{
        /// <summary>
        /// Primary residence number
        /// </summary>
        [EnumValue("Home")]
        PRN,

        /// <summary>
        /// Work number
        /// </summary>
        [EnumValue("Work")]
        WPN,
	}
}