#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
		[EnumValue("Unknown", Description = "Image availability has not been checked.")]
		X,

		/// <summary>
		/// Indeterminate
		/// </summary>
        [EnumValue("Indeterminate", Description = "The system does not have enough information to determine image availability.")]
		N,

		/// <summary>
		/// Zero
		/// </summary>
		[EnumValue("Zero", Description = "No images are available.")]
		Z,

		/// <summary>
		/// Partial
		/// </summary>
		[EnumValue("Partial", Description = "Some images are available, but not as many as expected.")]
		P,

		/// <summary>
		/// Complete
		/// </summary>
		[EnumValue("Complete", Description = "All images are available.")]
		C
	}
}