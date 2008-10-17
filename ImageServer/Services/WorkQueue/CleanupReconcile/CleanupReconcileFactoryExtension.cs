using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CleanupReconcile
{

    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class CleanupReconcileFactoryExtension : IWorkQueueProcessorFactory
    {
        #region Private Members
        #endregion

        #region Constructors
        public CleanupReconcileFactoryExtension()
        { }
        #endregion

        #region IWorkQueueProcessorFactory Members

        public WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.ReconcileCleanup ;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            CleanupReconcileItemProcessor processor = new CleanupReconcileItemProcessor();
            processor.Name = GetWorkQueueType().ToString();
            return processor;
        }

        #endregion
    }
}
