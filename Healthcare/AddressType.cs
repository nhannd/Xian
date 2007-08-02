using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// AddressType enumeration
    /// </summary>
    [EnumValueClass(typeof(AddressTypeEnum))]
	public enum AddressType
	{
        /// <summary>
        /// Business
        /// </summary>
        [EnumValue("Work")]
        B,

        /// <summary>
        /// Residential
        /// </summary>
        [EnumValue("Home")]
        R,

        /// <summary>
        /// Mailing
        /// </summary>
        [EnumValue("Mailing")]
        M
	}
}