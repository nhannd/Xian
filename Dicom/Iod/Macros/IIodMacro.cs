#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom.Iod.Macros
{
	public interface IIodMacro
	{
		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		void InitializeAttributes();

		/// <summary>
		/// Gets or sets the underlying DICOM sequence item.
		/// </summary>
		/// <remarks>
		/// This property may return NULL for macros implemented at the module level rather than on a sequence item.
		/// </remarks>
		/// <value>The DICOM sequence item.</value>
		DicomSequenceItem DicomSequenceItem { get; set; }
	}
}