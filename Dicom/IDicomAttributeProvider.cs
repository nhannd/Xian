#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom
{
	public delegate DicomAttribute DicomAttributeGetter(uint tag);
	public delegate void DicomAttributeSetter(uint tag, DicomAttribute value);
	
	/// <summary>
	/// Interface for classes that provide <see cref="DicomAttribute"/>s.
	/// </summary>
	public interface IDicomAttributeProvider
	{
		/// <summary>
		/// Gets or sets the <see cref="DicomAttribute"/> for the given tag.
		/// </summary>
		DicomAttribute this[DicomTag tag] { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="DicomAttribute"/> for the given tag.
		/// </summary>
		DicomAttribute this[uint tag] { get; set; }

		/// <summary>
		/// Attempts to get the attribute specified by <paramref name="tag"/>.
		/// </summary>
		bool TryGetAttribute(uint tag, out DicomAttribute attribute);

		/// <summary>
		/// Attempts to get the attribute specified by <paramref name="tag"/>.
		/// </summary>
		bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute);
	}
}