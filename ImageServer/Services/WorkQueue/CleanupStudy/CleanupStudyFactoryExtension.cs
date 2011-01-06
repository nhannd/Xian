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

namespace ClearCanvas.ImageServer.Services.WorkQueue.CleanupStudy
{
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class CleanupStudyFactoryExtension : IWorkQueueProcessorFactory
    {
        #region Private Members
        #endregion

        #region Constructors
        public CleanupStudyFactoryExtension()
        { }
        #endregion

        #region IWorkQueueProcessorFactory Members

        public WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.CleanupStudy;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            CleanupStudyItemProcessor processor= new CleanupStudyItemProcessor();
            processor.Name = WorkQueueTypeEnum.CleanupStudy.ToString();
            return processor;
        }

        #endregion
    }
}
