#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Core.Validation
{
    public enum ValidationErrors
    {
        // Object count in db and study xml do not match
        InconsistentObjectCount
    }

    /// <summary>
    /// Represents exception thrown when study validation fails.
    /// </summary>
    public class StudyIntegrityValidationFailure : Exception
    {
        #region Private Members
        private readonly ValidationErrors _error;
        private readonly ValidationStudyInfo _validationStudyInfo;
        #endregion
        #region Constructors

        public StudyIntegrityValidationFailure(ValidationErrors error, ValidationStudyInfo validationStudyInfo, string details)
            : base(details)
        {
            Platform.CheckForNullReference(validationStudyInfo, "validationStudyInfo");

            _error = error;
            _validationStudyInfo = validationStudyInfo;
        }
        
        #endregion

        #region Public Properties

        #endregion

        /// <summary>
        /// Gets the <see cref="ValidationStudyInfo"/> for the study that failed the validation.
        /// </summary>
        public ValidationStudyInfo ValidationStudyInfo
        {
            get { return _validationStudyInfo; }
        }

        /// <summary>
        /// Gets the <see cref="ValidationErrors"/>.
        /// </summary>
        public ValidationErrors Error
        {
            get { return _error; }
        }
    }
}