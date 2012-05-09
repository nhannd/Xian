#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    // Note, although this is specified as a Shred, we don't actual use it as a Shred
    // due to wanting it to be in the same app domain as the WorkItemActivityMonitorService
    internal class WorkItemProcessorExtension : QueueProcessorShred
    {
        public override string GetDisplayName()
        {
            return SR.WorkItemProcessorService;
        }

        public override string GetDescription()
        {
            return SR.WorkItemProcessorDescription;
        }

        protected override IList<QueueProcessor> GetProcessors()
        {
            WorkItemProcessor.CreateProcessor(WorkItemServiceSettings.Instance.StatThreadCount, WorkItemServiceSettings.Instance.NormalThreadCount, GetDisplayName());
            return new List<QueueProcessor> { WorkItemProcessor.Instance };
        }
    }
}
