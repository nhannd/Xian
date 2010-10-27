#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when the study is in invalid state.
    /// </summary>
    public class StudyIsInInvalidStateException: Exception
    {
        public StudyIsInInvalidStateException(StudyStorageLocation location, string message)
            :base(message)
        {
            CurrentState = location.StudyStatusEnum.Description;
            StudyInstanceUid = location.StudyInstanceUid;
        }

        public string CurrentState { get; set; }
        /// <summary>
        /// The study instance UID of the study that is nearline.
        /// </summary>		
        public string StudyInstanceUid { get; set; }
    }
}