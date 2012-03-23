#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class WorkItemProcessorExtension : QueueProcessorShred
    {
        public override string GetDisplayName()
        {
            return SR.WorkItemProcessorService;
        }

        public override string GetDescription()
        {
            return SR.WorkitemProcessorDescription;
        }

        protected override IList<QueueProcessor> GetProcessors()
        {
            return new List<QueueProcessor> {new WorkItemProcessor(WorkItemServiceSettings.Instance.StatThreadCount,WorkItemServiceSettings.Instance.NormalThreadCount,GetDisplayName())};
        }
    }
}
