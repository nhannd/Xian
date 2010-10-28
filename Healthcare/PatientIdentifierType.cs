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
    /// PatientIdentifierType enumeration - see HL7 2.9.12.5
    /// </summary>
	public enum PatientIdentifierType
	{
        /// <summary>
        /// MRN - Medical Record Number
        /// </summary>
        [EnumValue("MRN", Description="Medical Record Number")]
        MR,

        /// <summary>
        /// Healthcard Number
        /// </summary>
        [EnumValue("Healthcard")]
        HC,
	}
}