#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.ContextGroups
{
	public sealed class KeyObjectSelectionDocumentTitleContextGroup : ContextGroupBase<KeyObjectSelectionDocumentTitle>
	{
		private KeyObjectSelectionDocumentTitleContextGroup() : base(7010, "Key Object Selection Document Title", true, new DateTime(2004, 09, 20)) {}

		public static readonly KeyObjectSelectionDocumentTitle OfInterest = new KeyObjectSelectionDocumentTitle(113000, "Of Interest");
		public static readonly KeyObjectSelectionDocumentTitle RejectedForQualityReasons = new KeyObjectSelectionDocumentTitle(113001, "Rejected for Quality Reasons");
		public static readonly KeyObjectSelectionDocumentTitle ForReferringProvider = new KeyObjectSelectionDocumentTitle(113002, "For Referring Provider");
		public static readonly KeyObjectSelectionDocumentTitle ForSurgery = new KeyObjectSelectionDocumentTitle(113003, "For Surgery");
		public static readonly KeyObjectSelectionDocumentTitle ForTeaching = new KeyObjectSelectionDocumentTitle(113004, "For Teaching");
		public static readonly KeyObjectSelectionDocumentTitle ForConference = new KeyObjectSelectionDocumentTitle(113005, "For Conference");
		public static readonly KeyObjectSelectionDocumentTitle ForTherapy = new KeyObjectSelectionDocumentTitle(113006, "For Therapy");
		public static readonly KeyObjectSelectionDocumentTitle ForPatient = new KeyObjectSelectionDocumentTitle(113007, "For Patient");
		public static readonly KeyObjectSelectionDocumentTitle ForPeerReview = new KeyObjectSelectionDocumentTitle(113008, "For Peer Review");
		public static readonly KeyObjectSelectionDocumentTitle ForResearch = new KeyObjectSelectionDocumentTitle(113009, "For Research");
		public static readonly KeyObjectSelectionDocumentTitle QualityIssue = new KeyObjectSelectionDocumentTitle(113010, "Quality Issue");
		public static readonly KeyObjectSelectionDocumentTitle BestInSet = new KeyObjectSelectionDocumentTitle(113013, "Best In Set");
		public static readonly KeyObjectSelectionDocumentTitle ForPrinting = new KeyObjectSelectionDocumentTitle(113018, "For Printing");
		public static readonly KeyObjectSelectionDocumentTitle ForReportAttachment = new KeyObjectSelectionDocumentTitle(113020, "For Report Attachment");
		public static readonly KeyObjectSelectionDocumentTitle Manifest = new KeyObjectSelectionDocumentTitle(113030, "Manifest");
		public static readonly KeyObjectSelectionDocumentTitle SignedManifest = new KeyObjectSelectionDocumentTitle(113031, "Signed Manifest");
		public static readonly KeyObjectSelectionDocumentTitle CompleteStudyContent = new KeyObjectSelectionDocumentTitle(113032, "Complete Study Content");
		public static readonly KeyObjectSelectionDocumentTitle SignedCompleteStudyContent = new KeyObjectSelectionDocumentTitle(113033, "Signed Complete Study Content");
		public static readonly KeyObjectSelectionDocumentTitle CompleteAcquisitionContent = new KeyObjectSelectionDocumentTitle(113034, "Complete Acquisition Content");
		public static readonly KeyObjectSelectionDocumentTitle SignedCompleteAcquisitionContent = new KeyObjectSelectionDocumentTitle(113035, "Signed Complete Acquisition Content");
		public static readonly KeyObjectSelectionDocumentTitle GroupOfFramesForDisplay = new KeyObjectSelectionDocumentTitle(113036, "Group of Frames for Display");

		#region Singleton Instancing

		private static readonly KeyObjectSelectionDocumentTitleContextGroup _contextGroup = new KeyObjectSelectionDocumentTitleContextGroup();

		public static KeyObjectSelectionDocumentTitleContextGroup Instance
		{
			get { return _contextGroup; }
		}

		#endregion

		#region Static Enumeration of Values

		public static IEnumerable<KeyObjectSelectionDocumentTitle> Values
		{
			get
			{
				yield return OfInterest;
				yield return RejectedForQualityReasons;
				yield return ForReferringProvider;
				yield return ForSurgery;
				yield return ForTeaching;
				yield return ForConference;
				yield return ForTherapy;
				yield return ForPatient;
				yield return ForPeerReview;
				yield return ForResearch;
				yield return QualityIssue;
				yield return BestInSet;
				yield return ForPrinting;
				yield return ForReportAttachment;
				yield return Manifest;
				yield return SignedManifest;
				yield return CompleteStudyContent;
				yield return SignedCompleteStudyContent;
				yield return CompleteAcquisitionContent;
				yield return SignedCompleteAcquisitionContent;
				yield return GroupOfFramesForDisplay;
			}
		}

		/// <summary>
		/// Gets an enumerator that iterates through the defined titles.
		/// </summary>
		/// <returns>A <see cref="IEnumerator{T}"/> object that can be used to iterate through the defined titles.</returns>
		public override IEnumerator<KeyObjectSelectionDocumentTitle> GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		public static KeyObjectSelectionDocumentTitle LookupTitle(CodeSequenceMacro codeSequence)
		{
			return Instance.Lookup(codeSequence);
		}

		#endregion
	}

	/// <summary>
	/// Represents a key object selection document title.
	/// </summary>
	public sealed class KeyObjectSelectionDocumentTitle : ContextGroupBase<KeyObjectSelectionDocumentTitle>.ContextGroupItemBase
	{
		/// <summary>
		/// Constructor for titles defined in DICOM 2008, Part 16, Annex B, CID 7010.
		/// </summary>
		internal KeyObjectSelectionDocumentTitle(int value, string meaning) : base("DCM", value.ToString(), meaning) {}

		/// <summary>
		/// Constructs a new key object selection document title.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public KeyObjectSelectionDocumentTitle(string codingSchemeDesignator, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new key object selection document title.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codingSchemeVersion">The version of the coding scheme in which this code is defined, if known. Should be <code>null</code> if not explicitly specified.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public KeyObjectSelectionDocumentTitle(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codingSchemeVersion, codeValue, codeMeaning) {}
	}
}