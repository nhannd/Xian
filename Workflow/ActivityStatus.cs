using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
    [EnumValueClass(typeof(ActivityStatusEnum))]
    public enum ActivityStatus
    {
        /// <summary>
        /// Scheduled
        /// </summary>
        [EnumValue("Scheduled")]
        SC = 0,

        /// <summary>
        /// In Progress
        /// </summary>
        [EnumValue("In Progress")]
        IP = 1,

        /// <summary>
        /// Suspended
        /// </summary>
        [EnumValue("Suspended")]
        SU = 2,

        /// <summary>
        /// Completed
        /// </summary>
        [EnumValue("Completed")]
        CM = 3,

        /// <summary>
        /// Discontinued
        /// </summary>
        [EnumValue("Discontinued")]
        DC = 4
    }
}
