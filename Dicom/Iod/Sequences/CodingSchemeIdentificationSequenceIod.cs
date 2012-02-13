#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// CodingSchemeIdentification Sequence Item
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2009, Part 3, Section C.12.1 (Table C.12-1)</para>
	/// </remarks>
	public class CodingSchemeIdentificationSequenceItem
		: SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodingSchemeIdentificationSequence"/> class.
		/// </summary>
		public CodingSchemeIdentificationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodingSchemeIdentificationSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CodingSchemeIdentificationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of CodingSchemeDesignator in the underlying collection. Type 1.
		/// </summary>
		public string CodingSchemeDesignator
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeDesignator].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "CodingSchemeDesignator is Type 1 Required.");
				DicomAttributeProvider[DicomTags.CodingSchemeDesignator].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeRegistry in the underlying collection. Type 1C.
		/// </summary>
		public string CodingSchemeRegistry
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeRegistry].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeRegistry] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeRegistry].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeUid in the underlying collection. Type 1C.
		/// </summary>
		public string CodingSchemeUid
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeUid].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeUid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeUid].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeExternalId in the underlying collection. Type 2C.
		/// </summary>
		public string CodingSchemeExternalId
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeExternalId].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeExternalId] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeExternalId].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeName in the underlying collection. Type 3.
		/// </summary>
		public string CodingSchemeName
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeName] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeVersion in the underlying collection. Type 3.
		/// </summary>
		public string CodingSchemeVersion
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeVersion].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeVersion] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeVersion].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeResponsibleOrganization in the underlying collection. Type 3.
		/// </summary>
		public string CodingSchemeResponsibleOrganization
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeResponsibleOrganization].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeResponsibleOrganization] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeResponsibleOrganization].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="ClearCanvas.Dicom.DicomTag"/>s used by this sequence item.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.CodingSchemeDesignator;
				yield return DicomTags.CodingSchemeRegistry;
				yield return DicomTags.CodingSchemeUid;
				yield return DicomTags.CodingSchemeExternalId;
				yield return DicomTags.CodingSchemeName;
				yield return DicomTags.CodingSchemeVersion;
				yield return DicomTags.CodingSchemeResponsibleOrganization;
			}
		}
	}
}