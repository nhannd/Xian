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
    /// VisitLocationRole enumeration
    /// </summary>
    [EnumValueClass(typeof(VisitLocationRoleEnum))]
    public enum VisitLocationRole
	{
        /// <summary>
        /// Current
        /// </summary>
        [EnumValue("Current")]
        CR,

        /// <summary>
        /// Temporary
        /// </summary>
        [EnumValue("Temporary")]
        TM,

        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending")]
        PN,

        /// <summary>
        /// Prior
        /// </summary>
        [EnumValue("Prior")]
        PR
	}
}