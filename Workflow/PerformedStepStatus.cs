using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
    public enum PerformedStepStatus
    {
        /// <summary>
        /// In Progress
        /// </summary>
        [EnumValue("In Progress")]
        IP = 0,

        /// <summary>
        /// Completed
        /// </summary>
        [EnumValue("Completed")]
        CM = 1,

        /// <summary>
        /// Discontinued
        /// </summary>
        [EnumValue("Discontinued")]
        DC = 2
    }
}
