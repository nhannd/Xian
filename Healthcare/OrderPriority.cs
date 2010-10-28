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
    /// OrderPriority enumeration, defined by HL7 (see Timing Quantity TQ data-type)
    /// </summary>
    [EnumValueClass(typeof(OrderPriorityEnum))]
	public enum OrderPriority
	{
        /// <summary>
        /// Routine
        /// </summary>
        [EnumValue("Routine")]
        R,

        /// <summary>
        /// Urgent (or ASAP as defined by HL7)
        /// </summary>
        [EnumValue("Urgent")]
        A,

        /// <summary>
        /// Stat
        /// </summary>
        [EnumValue("Stat")]
        S
	}
}