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
    /// VisitStatus enumeration
    /// </summary>
    [EnumValueClass(typeof(VisitStatusEnum))]
    public enum VisitStatus
	{
        /// <summary>
        /// Pre-Admit
        /// </summary>
        [EnumValue("Pre-Admit")]
        PA,

        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending")]
        PD,

        /// <summary>
        /// Active/Admitted
        /// </summary>
        [EnumValue("Active/Admitted")]
        AA,

        /// <summary>
        /// Discharged
        /// </summary>
        [EnumValue("Discharged")]
        DC,

        /// <summary>
        /// Cancelled
        /// </summary>
        [EnumValue("Cancelled")]
        CX,

        /// <summary>
        /// Pre-Admit - Cancelled
        /// </summary>
        [EnumValue("Pre-Admit - Cancelled")]
        PC
    }
}