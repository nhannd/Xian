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
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    /// <summary>
    /// Plugin for processing 'ProcessDuplicate' WorkQueue items.
    /// </summary>
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class DuplicateProcessFactoryExtension : IWorkQueueProcessorFactory
    {
        #region Constructors
        public DuplicateProcessFactoryExtension()
        { }
        #endregion

        #region IWorkQueueProcessorFactory Members

        public virtual WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.ProcessDuplicate;
        }

        public virtual IWorkQueueItemProcessor GetItemProcessor()
        {
            ProcessDuplicateItemProcessor processor = new ProcessDuplicateItemProcessor();
            processor.Name = WorkQueueTypeEnum.ProcessDuplicate.ToString();
            return processor;
        }

        #endregion
    }
}