using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// TelephoneEquipment enumeration
    /// </summary>
	public enum TelephoneEquipment
	{
        /// <summary>
        /// Telephone
        /// </summary>
        [EnumValue("Telephone")]
        PH,

        /// <summary>
        /// Cell phone
        /// </summary>
        [EnumValue("Cellphone")]
        CP
	}
}