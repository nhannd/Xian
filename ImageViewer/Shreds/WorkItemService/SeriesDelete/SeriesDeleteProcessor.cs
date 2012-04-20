#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Dicom.Core;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.SeriesDelete
{

    public class SeriesDeleteProcessor : BaseItemProcessor<DeleteSeriesRequest, DeleteProgress>
    {
        public override bool Initialize(WorkItemStatusProxy proxy)
        {
            bool initResult = base.Initialize(proxy);
                       
            return initResult;
        }

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

            var deleteSeries = new DeleteSeriesUtility();
            deleteSeries.Initialize(Location, Request.SeriesInstanceUids);

            if (deleteSeries.DeletingAllSeries())
            {
                var deleteStudy = new DeleteStudyUtility();
                deleteStudy.Initialize(Location);
                Progress.ImagesToDelete = deleteStudy.NumberOfStudyRelatedInstances;
                Progress.ImagesDeleted = 0;
                Proxy.UpdateProgress();

                deleteStudy.Process();
            }
            else
            {
                Progress.ImagesToDelete = deleteSeries.NumberOfSeriesRelatedInstances;
                Progress.ImagesDeleted = 0;
                Proxy.UpdateProgress();

                deleteSeries.Process();
            }

            Progress.ImagesDeleted = Progress.ImagesToDelete;            

            Proxy.Complete();
        }

        public override bool CanStart(out string reason)
        {
            var relatedList = FindRelatedWorkItems(null, new List<WorkItemStatusEnum> { WorkItemStatusEnum.InProgress });

            reason = string.Empty;

            if (relatedList.Count > 0)
            {
                reason = "There are related WorkItems for the study being processed.";
                return false;
            }
            return true;
        }
    }
}
