#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// As per Dicom DOC 3 Table C.12-1
    /// </summary>
    public class SopCommonModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SopCommonModuleIod"/> class.
        /// </summary>
        public SopCommonModuleIod()
            :base()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SopCommonModuleIod"/> class.
        /// </summary>
		public SopCommonModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the sop class uid.
        /// </summary>
        /// <value>The sop class uid.</value>
        public string SopClassUid
        {
            get { return base.DicomAttributeProvider[DicomTags.SopClassUid].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.SopClassUid].SetStringValue(value); }
        }

        /// <summary>
        /// Gets or sets the sop class.
        /// </summary>
        /// <value>The sop class.</value>
        public SopClass SopClass
        {
            get { return SopClass.GetSopClass(SopClassUid); }
            set { SopClassUid = value.Uid; }
        }

        /// <summary>
        /// Gets or sets the sop instance uid.
        /// </summary>
        /// <value>The sop instance uid.</value>
        public string SopInstanceUid
        {
            get { return base.DicomAttributeProvider[DicomTags.SopInstanceUid].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.SopInstanceUid].SetStringValue(value); }
        }

        /// <summary>
        /// Gets or sets the specific character set.
        /// </summary>
        /// <value>The specific character set.</value>
        public string SpecificCharacterSet
        {
            get { return base.DicomAttributeProvider[DicomTags.SpecificCharacterSet].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.SpecificCharacterSet].SetStringValue(value); }
        }

        #endregion

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.SopClassUid;
				yield return DicomTags.SopInstanceUid;
				yield return DicomTags.SpecificCharacterSet;
			}
		}

    }
}
