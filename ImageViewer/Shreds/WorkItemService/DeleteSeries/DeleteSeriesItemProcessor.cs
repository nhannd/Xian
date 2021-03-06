﻿#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.DeleteSeries
{
    /// <summary>
    /// Class for processing requests to delete individual series within a study.
    /// </summary>
    internal class SeriesDeleteProcessor : BaseItemProcessor<DeleteSeriesRequest, DeleteProgress>
    {
        /// <summary>
        /// Process the Series delete request.
        /// </summary>
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
                var deleteSeries = new DeleteSeriesUtility();
                deleteSeries.Initialize(Location, Request.SeriesInstanceUids);

                if (deleteSeries.DeletingAllSeries())
                {
                    var deleteStudy = new DeleteStudyUtility();
                    deleteStudy.Initialize(Location);
                    Progress.IsCancelable = false;
                    Progress.TotalImagesToDelete = deleteStudy.NumberOfStudyRelatedInstances;
                    Progress.ImagesDeleted = 0;
                    Proxy.UpdateProgress();

                    deleteStudy.Process();
                }
                else
                {
                    Progress.IsCancelable = false;
                    Progress.TotalImagesToDelete = deleteSeries.NumberOfSeriesRelatedInstances;
                    Progress.ImagesDeleted = 0;
                    Proxy.UpdateProgress();

                    deleteSeries.Process();
                }

                Progress.ImagesDeleted = Progress.TotalImagesToDelete;

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
