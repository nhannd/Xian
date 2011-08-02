#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Enumerated values for the <see cref="DicomTags.ResponsiblePersonRole"/> attribute indicating the role of the responsible person for the named patient.
	/// </summary>
	/// <remarks>
	/// As defined in the DICOM Standard 2009, Part 3, Section C.7.1.1.1.2
	/// </remarks>
	public enum ResponsiblePersonRole
	{
		/// <summary>
		/// None.
		/// </summary>
		None,

		/// <summary>
		/// Owner.
		/// </summary>
		Owner,

		/// <summary>
		/// Parent.
		/// </summary>
		Parent,

		/// <summary>
		/// Child.
		/// </summary>
		Child,

		/// <summary>
		/// Spouse.
		/// </summary>
		Spouse,

		/// <summary>
		/// Sibling.
		/// </summary>
		Sibling,

		/// <summary>
		/// Relative.
		/// </summary>
		Relative,

		/// <summary>
		/// Guardian.
		/// </summary>
		Guardian,

		/// <summary>
		/// Custodian.
		/// </summary>
		Custodian,

		/// <summary>
		/// Agent.
		/// </summary>
		Agent
	}
}