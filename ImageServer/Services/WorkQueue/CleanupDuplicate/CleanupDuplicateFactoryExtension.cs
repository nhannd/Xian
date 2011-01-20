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

namespace ClearCanvas.ImageServer.Services.WorkQueue.CleanupDuplicate
{
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class CleanupDuplicateFactoryExtension : IWorkQueueProcessorFactory
    {
        #region Private Members
        #endregion

        #region Constructors
        public CleanupDuplicateFactoryExtension()
        { }
        #endregion

        #region IWorkQueueProcessorFactory Members

        public WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.CleanupDuplicate;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            CleanupDuplicateItemProcessor processor = new CleanupDuplicateItemProcessor();
            processor.Name = GetWorkQueueType().ToString();
            return processor;
        }

        #endregion
    }
}