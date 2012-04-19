#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.ImageViewer.Common.WorkItem;

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
            
            // Reset progress, in case of retry
            Progress.IsCancelable = false;
            Progress.ImagesDeleted = 0;
            Progress.ImagesToDelete = 0;

            Proxy.UpdateProgress();

            Proxy.Complete();
        }

        public override bool CanStart(out string reason)
        {            
            reason = string.Empty;
            return !InProgressWorkItems();
        }
    }
}
