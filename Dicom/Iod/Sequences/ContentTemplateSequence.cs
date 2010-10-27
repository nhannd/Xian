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
	/// Content Template Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.8 (Table C.18.8-1)</remarks>
	public class ContentTemplateSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContentTemplateSequence"/> class.
		/// </summary>
		public ContentTemplateSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContentTemplateSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ContentTemplateSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		public void InitializeAttributes()
		{
			this.MappingResource = "DCMR";
			this.TemplateIdentifier = "1";
		}

		/// <summary>
		/// Gets or sets the value of MappingResource in the underlying collection. Type 1.
		/// </summary>
		public string MappingResource
		{
			get { return base.DicomAttributeProvider[DicomTags.MappingResource].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "MappingResource is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.MappingResource].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of TemplateIdentifier in the underlying collection. Type 1.
		/// </summary>
		public string TemplateIdentifier
		{
			get { return base.DicomAttributeProvider[DicomTags.TemplateIdentifier].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "Template Identifier is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.TemplateIdentifier].SetString(0, value);
			}
		}
	}
}