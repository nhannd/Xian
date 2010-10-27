#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Common.Exceptions
{
    /// <summary>
    /// Represents an exception that occured during auto-reconciliation
    /// is nearline.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class AutoReconcileException : Exception
    {
        public AutoReconcileException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Represents an exception that occured when the target study during auto-reconciliation
    /// is not in the right state (eg, it is lossy compressed but has been archived as lossless).
    /// </summary>
    public class TargetStudyInvalidStateException: AutoReconcileException
    {
        public TargetStudyInvalidStateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// The Study Instance UID of the study that causes the issue.
        /// </summary>
        public string StudyInstanceUid { get; set; }
    }
}