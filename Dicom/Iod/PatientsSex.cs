#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Enumerated values for the <see cref="DicomTags.PatientsSex"/> attribute indicating the sex of the named patient.
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.1 (Table C.7-1)</remarks>
	public enum PatientsSex
	{
		/// <summary>
		/// male.
		/// </summary>
		M,

		/// <summary>
		/// female.
		/// </summary>
		F,

		/// <summary>
		/// other.
		/// </summary>
		O,

		/// <summary>
		/// Represents the null value.
		/// </summary>
		None
	}
}
