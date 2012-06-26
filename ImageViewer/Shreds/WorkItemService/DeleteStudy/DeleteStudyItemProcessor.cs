#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.DeleteStudy
{
    /// <summary>
    /// Class for processing requests to delete studies.
    /// </summary>
    internal class DeleteStudyItemProcessor : BaseItemProcessor<DeleteStudyRequest, DeleteProgress>
    {        
        public override void Process()
        {
            if (CancelPending)
            {
                Proxy.Cancel();
                return;
            }
            if (StopPending)
            {
                Proxy.Postpone();
                return;
            }

            try
            {
                // Reset progress, in case of retry
                Progress.IsCancelable = false;
                Progress.ImagesDeleted = 0;
                Progress.TotalImagesToDelete = 0;

                var delete = new DeleteStudyUtility();

                delete.Initialize(Location);

                Progress.TotalImagesToDelete = delete.NumberOfStudyRelatedInstances;
                Proxy.UpdateProgress();

                delete.Process();
                Progress.ImagesDeleted = delete.NumberOfStudyRelatedInstances;

                Proxy.Complete();
            }
            catch (Exception)
            {
                Progress.IsCancelable = true;
                throw;
            }
        }
    }
}
