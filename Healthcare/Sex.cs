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

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// HL7 Sex enumeration
    /// </summary>
    [EnumValueClass(typeof(SexEnum))]
    public enum Sex
	{
        /// <summary>
        /// Unknown
        /// </summary>
        [EnumValue("Unknown")]
        U,

        /// <summary>
        /// Female
        /// </summary>
        [EnumValue("Female")]
        F,

        /// <summary>
        /// Male
        /// </summary>
        [EnumValue("Male")]
        M,

        /// <summary>
        /// Other
        /// </summary>
        [EnumValue("Other")]
        O
	}
}