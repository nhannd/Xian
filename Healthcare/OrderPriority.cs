#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// OrderPriority enumeration, defined by HL7 (see Timing Quantity TQ data-type)
    /// </summary>
    // NOTE: the numeric values are used for ordering and must not be modified! (Modification will break existing installations)
    [EnumValueClass(typeof(OrderPriorityEnum))]
	public enum OrderPriority
	{
        /// <summary>
        /// Routine
        /// </summary>
        [EnumValue("Routine")]
        R = 0,

        /// <summary>
        /// Urgent (or ASAP as defined by HL7)
        /// </summary>
        [EnumValue("Urgent")]
        A = 1,

        /// <summary>
        /// Stat
        /// </summary>
        [EnumValue("Stat")]
        S = 2
	}
}