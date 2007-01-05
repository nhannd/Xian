using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Workflow
{
    public enum ActivityStatus
    {
        /// <summary>
        /// Scheduled
        /// </summary>
        [EnumValue("Scheduled")]
        SC,

        /// <summary>
        /// In Progress
        /// </summary>
        [EnumValue("In Progress")]
        IP,

        /// <summary>
        /// Suspended
        /// </summary>
        [EnumValue("Suspended")]
        SU,

        /// <summary>
        /// Completed
        /// </summary>
        [EnumValue("Completed")]
        CM,

        /// <summary>
        /// Discontinued
        /// </summary>
        [EnumValue("Discontinued")]
        DC
    }
}
