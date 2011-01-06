#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Detailed view of an Auto-route <see cref="WorkQueue"/> item in the context of a WorkQueue details page.
    /// </summary>
    public class AutoRouteWorkQueueDetails : WorkQueueDetails
    {
        #region Private members

        #endregion Private members

        #region Public Properties

        public string DestinationAE { get; set; }

        public string StudyInstanceUid { get; set; }

        #endregion Public Properties
    }
}