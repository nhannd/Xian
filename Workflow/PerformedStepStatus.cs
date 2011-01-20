#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
    [EnumValueClass(typeof(PerformedStepStatusEnum))]
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
