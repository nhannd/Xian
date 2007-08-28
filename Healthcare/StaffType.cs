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