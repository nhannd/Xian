#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// ReferencedSopInstanceMac Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.17.2.1 (Table C.17-3a)</remarks>
	public class ReferencedSopInstanceMacSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencedSopInstanceMacSequence"/> class.
		/// </summary>
		public ReferencedSopInstanceMacSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencedSopInstanceMacSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ReferencedSopInstanceMacSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of MacCalculationTransferSyntaxUid in the underlying collection. Type 1.
		/// </summary>
		public string MacCalculationTransferSyntaxUid
		{
			get { return base.DicomAttributeProvider[DicomTags.MacCalculationTransferSyntaxUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "MacCalculationTransferSyntaxUid is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.MacCalculationTransferSyntaxUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of MacAlgorithm in the underlying collection. Type 1.
		/// </summary>
		public MacAlgorithm MacAlgorithm
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.MacAlgorithm].GetString(0, string.Empty), MacAlgorithm.Unknown); }
			set
			{
				if (value == MacAlgorithm.Unknown)
					throw new ArgumentOutOfRangeException("value", "MacAlgorithm is Type 1 Required.");
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.MacAlgorithm], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DataElementsSigned in the underlying collection. Type 1.
		/// </summary>
		public uint[] DataElementsSigned
		{
			get { return (uint[]) base.DicomAttributeProvider[DicomTags.DataElementsSigned].Values; }
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "DataElementsSigned is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.DataElementsSigned].Values = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of Mac in the underlying collection. Type 1.
		/// </summary>
		public byte[] Mac
		{
			get { return (byte[]) base.DicomAttributeProvider[DicomTags.Mac].Values; }
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "Mac is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.Mac].Values = value;
			}
		}
	}
}