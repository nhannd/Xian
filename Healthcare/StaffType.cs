#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// StaffType enumeration for purposes of categorizing staff.  This categorization may be used
    /// to make workflow decisions, but under no circumstances may it be used to for authorization
    /// decisions.  
    /// </summary>
    [EnumValueClass(typeof(StaffTypeEnum))]
    public enum StaffType
	{
        /// <summary>
        /// Unspecified
        /// </summary>
        [EnumValue("Unspecified", Description="Unspecified or unknown or not applicable")]
        X,

        /// <summary>
        /// Radiologist (Staff)
        /// </summary>
        [EnumValue("Radiologist", Description="Staff Radiologist")]
        PRAD,

        /// <summary>
        /// Radiology Resident
        /// </summary>
        [EnumValue("Resident", Description="Radiology Resident or non-staff radiologist")]
        PRAR,

        /// <summary>
        /// Referring Physician
        /// </summary>
        [EnumValue("Emergency Physician", Description = "Emergency Physician")]
        PEMR,

        /// <summary>
        /// Technologist
        /// </summary>
        [EnumValue("Technologist", Description="Radiology Technologist")]
        STEC,

        /// <summary>
        /// Transcriptionist
        /// </summary>
        [EnumValue("Transcriptionist", Description="Transcriptionist")]
        STRA,

        /// <summary>
        /// Clerical
        /// </summary>
        [EnumValue("Clerical", Description="Clerical")]
        SCLR,

        /// <summary>
        /// Healthcare Administrator
        /// </summary>
        [EnumValue("Healthcare Admin", Description="Healthcare Administrative staff")]
        SADH,

        /// <summary>
        /// System/Technical Administrator
        /// </summary>
        [EnumValue("System Admin", Description="System or Technical Administrative staff")]
        SADS,

        /// <summary>
        /// System/Technical Support
        /// </summary>
        [EnumValue("Technical Support", Description = "System or Technical Support staff")]
        SSUP
	}
}