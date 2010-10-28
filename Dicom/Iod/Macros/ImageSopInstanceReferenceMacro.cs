#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// Image SOP Instance Reference Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section 10.3 (Table 10-3)</remarks>
	public class ImageSopInstanceReferenceMacro : SequenceIodBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageSopInstanceReferenceMacro"/> class.
		/// </summary>
		public ImageSopInstanceReferenceMacro() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageSopInstanceReferenceMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ImageSopInstanceReferenceMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		#endregion

		#region Public Properties

		/// <summary>
		/// Uniquely identifies the referenced SOP Class
		/// </summary>
		/// <value>The referenced sop class uid.</value>
		public string ReferencedSopClassUid
		{
			get { return base.DicomAttributeProvider[DicomTags.ReferencedSopClassUid].GetString(0, String.Empty); }
			set { base.DicomAttributeProvider[DicomTags.ReferencedSopClassUid].SetString(0, value); }
		}

		/// <summary>
		/// Uniquely identifies the referenced SOP Instance.
		/// </summary>
		/// <value>The referenced sop instance uid.</value>
		public string ReferencedSopInstanceUid
		{
			get { return base.DicomAttributeProvider[DicomTags.ReferencedSopInstanceUid].GetString(0, String.Empty); }
			set { base.DicomAttributeProvider[DicomTags.ReferencedSopInstanceUid].SetString(0, value); }
		}

		/// <summary>
		/// Identifies the frame numbers within the Referenced SOP Instance to which the 
		/// reference applies. The first frame shall be denoted as frame number 1. 
		/// <para>Note: This Attribute may be multi-valued. </para> 
		/// <para>
		/// Required if the Referenced SOP Instance is a multi-frame image and the reference 
		/// does not apply to all frames, and Referenced Segment Number (0062,000B) is not present.
		/// </para> 
		/// </summary>
		/// <value>The referenced frame number.</value>
		public DicomAttributeIS ReferencedFrameNumber
		{
			get { return base.DicomAttributeProvider[DicomTags.ReferencedFrameNumber] as DicomAttributeIS; }
		}

		/// <summary>
		/// Identifies the Segment Number to which the reference applies. Required if the Referenced
		///  SOP Instance is a Segmentation and the reference does not apply to all segments and
		///  Referenced Frame Number (0008,1160) is not present.
		/// </summary>
		/// <value>The referenced segment number.</value>
		public DicomAttributeUS ReferencedSegmentNumber
		{
			get { return base.DicomAttributeProvider[DicomTags.ReferencedSegmentNumber] as DicomAttributeUS; }
		}

		#endregion
	}
}