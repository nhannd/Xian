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

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy
{
    /// <summary>
    /// Plugin for processing 'DeleteStudy' WorkQueue items.
    /// </summary>
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class DeleteStudyFactoryExtension : IWorkQueueProcessorFactory
    {
        #region Constructors
        public DeleteStudyFactoryExtension()
        { }
        #endregion

        #region IWorkQueueProcessorFactory Members

        public virtual WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.DeleteStudy;
        }

        public virtual IWorkQueueItemProcessor GetItemProcessor()
        {
            DeleteStudyItemProcessor processor = new DeleteStudyItemProcessor();
            processor.Name = WorkQueueTypeEnum.DeleteStudy.ToString();
            return processor;
        }

        #endregion
    }
}
