#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// DeIdentificationMethod Code Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.1 (Table C.7-1)</remarks>
	public class DeIdentificationMethodCodeSequence : CodeSequenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DeIdentificationMethodCodeSequence"/> class.
		/// </summary>
		public DeIdentificationMethodCodeSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeIdentificationMethodCodeSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public DeIdentificationMethodCodeSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}
	}
}
