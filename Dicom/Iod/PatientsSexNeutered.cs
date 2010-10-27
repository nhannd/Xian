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
	/// Enumerated values for the <see cref="DicomTags.PatientsSexNeutered"/> attribute describing whether or not a procedure has been performed in an effort to render the patient sterile.
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.2.2 (Table C.7-4a)</remarks>
	public enum PatientsSexNeutered
	{
		/// <summary>
		/// Altered/Neutered.
		/// </summary>
		Altered,

		/// <summary>
		/// Unaltered/Intact.
		/// </summary>
		Unaltered,

		/// <summary>
		/// Represents the unknown status, which is equivalent to the null value.
		/// </summary>
		Unknown
	}
}