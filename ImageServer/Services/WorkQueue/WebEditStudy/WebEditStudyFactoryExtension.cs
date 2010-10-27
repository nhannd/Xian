#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class WebEditStudyFactoryExtension : IWorkQueueProcessorFactory
    {
        #region IWorkQueueProcessorFactory Members

        public virtual WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.WebEditStudy;
        }

        public virtual IWorkQueueItemProcessor GetItemProcessor()
        {
            WebEditStudyItemProcessor processor = new WebEditStudyItemProcessor();
            processor.Name = WorkQueueTypeEnum.WebEditStudy.ToString();
            return processor;
        }

        #endregion
    }
}
