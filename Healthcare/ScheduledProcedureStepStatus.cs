using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// ScheduledProcedureStepStatus enumeration.  This enumeration is defined by DICOM (0040, 4001)
    /// </summary>
	public enum ScheduledProcedureStepStatus
	{
        /// <summary>
        /// Scheduled
        /// </summary>
        [EnumValue("Scheduled")]
        SCHEDULED,

        /// <summary>
        /// In Progress
        /// </summary>
        [EnumValue("In Progress")]
        INPROGRESS,

        /// <summary>
        /// Suspended
        /// </summary>
        [EnumValue("Suspended")]
        SUSPENDED,

        /// <summary>
        /// Completed
        /// </summary>
        [EnumValue("Completed")]
        COMPLETED,

        /// <summary>
        /// Discontinued
        /// </summary>
        [EnumValue("Discontinued")]
        DISCONTINUED
	}
}