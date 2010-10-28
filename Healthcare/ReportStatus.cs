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

    [EnumValueClass(typeof(ReportStatusEnum))]
    public enum ReportStatus
	{
        /// <summary>
        /// Draft
        /// </summary>
        [EnumValue("Draft", Description = "Draft")]
        D,

        /// <summary>
        /// Preliminary
        /// </summary>
        [EnumValue("Preliminary", Description = "Preliminary (report has not been verified)")]
        P,
 
        /// <summary>
        /// Final
        /// </summary>
        [EnumValue("Final", Description = "Final (report has been verified)")]
        F,

        /// <summary>
        /// Corrected
        /// </summary>
        [EnumValue("Corrected", Description = "Corrected (report has one or more addenda)")]
        C,

        /// <summary>
        /// Cancelled
        /// </summary>
        [EnumValue("Cancelled", Description = "Report is cancelled")]
        X
	}
}