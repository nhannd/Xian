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
		X,

		/// <summary>
		/// Not Available
		/// </summary>
		[EnumValue("Not Available")]
		N,

		/// <summary>
		/// Zero
		/// </summary>
		[EnumValue("Zero")]
		Z,

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