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
    /// ProcedureStatus enumeration
    /// </summary>
    [EnumValueClass(typeof(ProcedureStatusEnum))]
	public enum ProcedureStatus
	{
        /// <summary>
        /// Scheduled
        /// </summary>
        [EnumValue("Scheduled", Description = "In process, scheduled")]
        SC,

        /// <summary>
        /// Canceled
        /// </summary>
        [EnumValue("Cancelled", Description = "Procedure was cancelled")]
        CA,

        /// <summary>
        /// Completed
        /// </summary>
        [EnumValue("Completed", Description = "Procedure was completed, including report publishing")]
        CM,

        /// <summary>
        /// Discontinued
        /// </summary>
        [EnumValue("Discontinued", Description = "Procedure was discontinued")]
        DC,

        /// <summary>
        /// In Progress
        /// </summary>
        [EnumValue("In Progress", Description = "In process, unspecified")]
        IP,

		/// <summary>
		/// Ghost
		/// </summary>
		[EnumValue("Ghost", Description = "Ghost of procedure that was moved to another order")]
		GH,
	}
}