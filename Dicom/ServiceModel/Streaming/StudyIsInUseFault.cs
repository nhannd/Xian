#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    /// <summary>
    /// Fault contract indicating the requested study cannot be accessed because it is being used on the server.
    /// </summary>
    [DataContract]
    public class StudyIsInUseFault
    {
        #region Private Members
        private String _studyState;
        #endregion

        #region Constructors
        public StudyIsInUseFault(string state)
        {
            _studyState = state;
        }
        #endregion

        /// <summary>
        /// Gets or sets the current state of the study.
        /// </summary>
        /// 
        [DataMember(IsRequired = false)]
        public String StudyState
        {
            get { return _studyState; }
            set { _studyState = value; }
        }
    }
}