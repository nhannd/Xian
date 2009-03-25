using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudyPostProcessing
{
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    class ReconcilePostProcessingProcessorFactoryExtension : IWorkQueueProcessorFactory
    {
        public WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.ReconcilePostProcess;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            ReconcilePostProcessingProcessor processor = new ReconcilePostProcessingProcessor();
            processor.Name = WorkQueueTypeEnum.ReconcilePostProcess.ToString();
            return processor;
        }
    }
}
