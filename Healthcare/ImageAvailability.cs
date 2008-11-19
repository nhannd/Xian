using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
	/// ImageAvailability enumeration
    /// </summary>
    [EnumValueClass(typeof(ImageAvailabilityEnum))]
	public enum ImageAvailability
	{
		/// <summary>
		/// Unknown
		/// </summary>
		[EnumValue("Unknown")]
		U,

		/// <summary>
		/// None
		/// </summary>
		[EnumValue("None")]
		N,

		/// <summary>
		/// Partial
		/// </summary>
		[EnumValue("Partial")]
		P,

		/// <summary>
		/// Complete
		/// </summary>
		[EnumValue("Complete")]
		C
	}
}