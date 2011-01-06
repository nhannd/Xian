#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReprocessStudy
{
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    class ReprocessStudyFactoryExtension : IWorkQueueProcessorFactory
    {
        public WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.ReprocessStudy;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            ReprocessStudyItemProcessor processor = new ReprocessStudyItemProcessor();
            processor.Name = WorkQueueTypeEnum.ReprocessStudy.ToString();
            return processor;
        }
    }
}
