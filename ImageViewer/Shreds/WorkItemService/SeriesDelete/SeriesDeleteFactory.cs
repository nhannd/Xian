#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.SeriesDelete
{

    [ExtensionOf(typeof(WorkItemFactoryExtensionPoint))]
    public class SeriesDeleteFactory : IWorkItemProcessorFactory
    {
        public WorkItemTypeEnum GetWorkQueueType()
        {
            return WorkItemTypeEnum.SeriesDelete;
        }

        public IWorkItemProcessor GetItemProcessor()
        {
            return new SeriesDeleteProcessor();
        }
    }
}
