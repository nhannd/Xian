using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// RequestedProcedureStatus enumeration
    /// </summary>
    [EnumValueClass(typeof(RequestedProcedureStatusEnum))]
	public enum RequestedProcedureStatus
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
    }
}