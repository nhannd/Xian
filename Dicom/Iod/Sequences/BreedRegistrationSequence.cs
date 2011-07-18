#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// BreedRegistration Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.1 (Table C.7-1)</remarks>
	public class BreedRegistrationSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BreedRegistrationSequence"/> class.
		/// </summary>
		public BreedRegistrationSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="BreedRegistrationSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public BreedRegistrationSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.BreedRegistrationNumber = " ";
		}

		/// <summary>
		/// Gets or sets the value of BreedRegistrationNumber in the underlying collection. Type 1.
		/// </summary>
		public string BreedRegistrationNumber
		{
			get { return base.DicomAttributeProvider[DicomTags.BreedRegistrationNumber].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "BreedRegistrationNumber is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.BreedRegistrationNumber].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of BreedRegistryCodeSequence in the underlying collection. Type 1.
		/// Only one item shall be present.
		/// </summary>
		public BreedRegistry BreedRegistryCodeSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.BreedRegistryCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					return null;

				var dicomSequenceItem = ((DicomSequenceItem[])dicomAttribute.Values)[0];
				return new BreedRegistry(new CodeSequenceMacro(dicomSequenceItem));
			}
			set
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.BreedRegistryCodeSequence];
				if (value == null)
					throw new ArgumentNullException("value", "BreedRegistryCodeSequence is Type 1 Required.");

				var sequenceItem = new CodeSequenceMacro();
				value.WriteToCodeSequence(sequenceItem);

				dicomAttribute.Values = new DicomSequenceItem[] { sequenceItem.DicomSequenceItem };
			}
		}
	}
}
