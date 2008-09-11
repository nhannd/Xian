using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    class ReconcileStudyFactoryExtension : IWorkQueueProcessorFactory
    {
        public WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.ReconcileStudy;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            ReconcileStudyItemProcessor processor = new ReconcileStudyItemProcessor();
            processor.Name = WorkQueueTypeEnum.ReconcileStudy.ToString();
            return processor;
        }
    }
}
