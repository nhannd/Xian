#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// List of recovery mechanisms used by different work queue processors when the entry
    /// is failed because of mismatch number of instances in the study xml and the database.
    /// </summary>
    public enum RecoveryModes
    {
        /// <summary>
        /// Users will handle it manually.
        /// </summary>
        Manual,

        /// <summary>
        /// The server will trigger a reprocess of the study.
        /// </summary>
        Automatic
    }
}