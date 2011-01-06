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
