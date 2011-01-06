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

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebMoveStudy
{
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class WebMoveStudyFactoryExtension: IWorkQueueProcessorFactory
    {
        #region IWorkQueueProcessorFactory Members

        public virtual WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.WebMoveStudy;
        }

        public virtual IWorkQueueItemProcessor GetItemProcessor()
        {
            WebMoveStudyItemProcessor processor = new WebMoveStudyItemProcessor();
            processor.Name = WorkQueueTypeEnum.WebMoveStudy.ToString();
            return processor;
        }

        #endregion
    }
}
