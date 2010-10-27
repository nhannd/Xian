#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageServer.Common.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when the study is nearline.
    /// </summary>
    public class StudyIsNearlineException : SopInstanceProcessingException
    {
        private readonly bool _restoreRequested;

        public StudyIsNearlineException(bool restoreRequested) : base("Study is in Nearline state.")
        {
            _restoreRequested = restoreRequested;
        }

        public bool RestoreRequested
        {
            get { return _restoreRequested; }
        }

		/// <summary>
        /// The study instance UID of the study that is nearline.
        /// </summary>		
        public string StudyInstanceUid { get; set; }

    }
}