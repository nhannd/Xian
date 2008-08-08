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
