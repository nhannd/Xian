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
using System.Linq;
using System.Text;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Dicom.Core;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.StudyDelete
{
    public class StudyDeleteProcessor : BaseItemProcessor<DeleteStudyRequest, DeleteProgress>
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

            var delete = new DeleteStudyUtility();

            delete.Initialize(Location);

            Progress.ImagesToDelete = delete.NumberOfStudyRelatedInstances;
            Proxy.UpdateProgress();

            delete.Process();
            Progress.ImagesDeleted = delete.NumberOfStudyRelatedInstances;

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