using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Workflow
{
    public enum ActivityPerformedStepStatus
    {
        /// <summary>
        /// In Progress
        /// </summary>
        [EnumValue("In Progress")]
        IP,

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
