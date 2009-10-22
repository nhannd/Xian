#region License

// Copyright (c) 2009, ClearCanvas Inc.
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