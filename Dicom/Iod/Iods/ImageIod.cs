#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// Generic Image IOD.  Note, in progress.
    /// </summary>
    public class ImageIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageIod"/> class.
        /// </summary>
        public ImageIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageIod"/> class.
        /// </summary>
        public ImageIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the patient module.
        /// </summary>
        /// <value>The patient module.</value>
        public PatientIdentificationModuleIod PatientIdentificationModule
        {
            get { return base.GetModuleIod<PatientIdentificationModuleIod>(); }
        }

        /// <summary>
        /// Gets the study module.
        /// </summary>
        /// <value>The study module.</value>
        public StudyModuleIod StudyModule
        {
            get { return base.GetModuleIod<StudyModuleIod>(); }
        }
        #endregion

    }
}
