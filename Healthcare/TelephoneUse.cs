#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
    /// TelephoneUse enumeration
    /// </summary>
    [EnumValueClass(typeof(TelephoneUseEnum))]
    public enum TelephoneUse
	{
        /// <summary>
        /// Primary residence number
        /// </summary>
        [EnumValue("Home")]
        PRN,

        /// <summary>
        /// Work number
        /// </summary>
        [EnumValue("Work")]
        WPN,
	}
}