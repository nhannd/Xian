#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// Specimen Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.2 (Table C.7-2a)</remarks>
	public class SpecimenSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SpecimenSequence"/> class.
		/// </summary>
		public SpecimenSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpecimenSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public SpecimenSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of SpecimenIdentifier in the underlying collection. Type 2.
		/// </summary>
		public string SpecimenIdentifier
		{
			get { return base.DicomAttributeProvider[DicomTags.SpecimenIdentifier].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.SpecimenIdentifier].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.SpecimenIdentifier].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SpecimenTypeCodeSequence in the underlying collection. Type 2C.
		/// </summary>
		public SpecimenTypeCodeSequence SpecimenTypeCodeSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.SpecimenTypeCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new SpecimenTypeCodeSequence(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.SpecimenTypeCodeSequence];
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.SpecimenTypeCodeSequence] = null;
					return;
				}
				dicomAttribute.Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of SlideIdentifier in the underlying collection. Type 2C.
		/// </summary>
		public string SlideIdentifier
		{
			get { return base.DicomAttributeProvider[DicomTags.SlideIdentifierRetired].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.SlideIdentifierRetired] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.SlideIdentifierRetired].SetString(0, value);
			}
		}
	}
}