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
	/// Enumerated values for the <see cref="ClearCanvas.Dicom.DicomTags.VerificationFlag"/> attribute indicating whether the Encapsulated Document is verified.
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2009, Part 3, Section C.24.2 (Table C.24-2)</para>
	/// </remarks>
	public enum VerificationFlag
	{
		/// <summary>
		/// Represents the null value.
		/// </summary>
		None,

		/// <summary>
		/// Indicates that the Encapsulated Document is not attested by a legally accountable person.
		/// </summary>
		Unverified,

		/// <summary>
		/// Indicates that the Encapsulated Document is attested to (signed) by a Verifying Observer
		/// or Legal Authenticator named in the document, who is accountable for its content.
		/// </summary>
		Verified
	}
}