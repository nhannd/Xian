using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// ContactPersonType enumeration
    /// </summary>
	public enum ContactPersonType
	{
        /// <summary>
        /// 
        /// </summary>
        [EnumValue("Next of Kin", Description = "Next of Kin")]
        NK,

        /// <summary>
        /// 
        /// </summary>
        [EnumValue("Emergency Contact", Description = "Emergency Contact")]
        EC
	}
}