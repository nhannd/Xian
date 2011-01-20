#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
    /// OrderStatus enumeration as defined by HL7 (4.5.1.5).  This is only a subset of what HL7 defines.
    /// More values can be added later if necessary.
    /// </summary>
    [EnumValueClass(typeof(OrderStatusEnum))]
    public enum OrderStatus
	{
        /// <summary>
        /// Scheduled
        /// </summary>
        [EnumValue("Scheduled", Description="In process, scheduled")]
        SC,
 
        /// <summary>
        /// Canceled
        /// </summary>
        [EnumValue("Canceled", Description="Order was canceled")]
        CA,

        /// <summary>
        /// Completed
        /// </summary>
        [EnumValue("Completed", Description="Order is completed")]
        CM,

        /// <summary>
        /// Discontinued
        /// </summary>
        [EnumValue("Discontinued", Description="Order was discontinued")]
        DC,

        /// <summary>
        /// In Progress
        /// </summary>
        [EnumValue("In Progress", Description="In process, unspecified")]
        IP,

        /// <summary>
        /// Replaced
        /// </summary>
        [EnumValue("Replaced", Description="Order was replaced")]
        RP,

		/// <summary>
		/// Merged
		/// </summary>
		[EnumValue("Merged", Description = "Order was merged into another order")]
		MG,
	}
}