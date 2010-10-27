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
    /// Laterality enumeration
    /// </summary>
    [EnumValueClass(typeof(LateralityEnum))]
	public enum Laterality
	{
        /// <summary>
        /// None
        /// </summary>
        [EnumValue("None", Description = "Not applicable")]
        N,

        /// <summary>
        /// Right
        /// </summary>
        [EnumValue("Right", Description = "Right")]
        R,

        /// <summary>
        /// Left
        /// </summary>
        [EnumValue("Left", Description = "Left")]
        L,

        /// <summary>
        /// Bilateral
        /// </summary>
        [EnumValue("Bilateral", Description = "Bilateral")]
        B
	}
}