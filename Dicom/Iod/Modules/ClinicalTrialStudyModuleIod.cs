#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// ClinicalTrialStudy Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.2.3 (Table C.7-4b)</remarks>
	public class ClinicalTrialStudyModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ClinicalTrialStudyModuleIod"/> class.
		/// </summary>	
		public ClinicalTrialStudyModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClinicalTrialStudyModuleIod"/> class.
		/// </summary>
		public ClinicalTrialStudyModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.ClinicalTrialTimePointId = null;
			this.ClinicalTrialTimePointDescription = null;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			if (string.IsNullOrEmpty(this.ClinicalTrialTimePointId) && string.IsNullOrEmpty(this.ClinicalTrialTimePointDescription))
				return false;
			return true;
		}

		/// <summary>
		/// Gets or sets the value of ClinicalTrialTimePointId in the underlying collection. Type 2.
		/// </summary>
		public string ClinicalTrialTimePointId
		{
			get { return base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointId].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ClinicalTrialTimePointDescription in the underlying collection. Type 3.
		/// </summary>
		public string ClinicalTrialTimePointDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointDescription].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointDescription] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointDescription].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.ClinicalTrialTimePointDescription;
				yield return DicomTags.ClinicalTrialTimePointId;
			}
		}
	}
}