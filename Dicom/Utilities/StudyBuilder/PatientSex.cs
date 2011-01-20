#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// An enumeration representing the values of the Patient's Sex DICOM attribute (Tag 0010,0040).
	/// </summary>
	public enum PatientSex
	{
		/// <summary>
		/// Represents unrecognized and empty code strings.
		/// </summary>
		Undefined,

		/// <summary>
		/// Represents the M code string.
		/// </summary>
		Male,

		/// <summary>
		/// Represents the F code string.
		/// </summary>
		Female,

		/// <summary>
		/// Represents the O code string.
		/// </summary>
		Other
	}
}