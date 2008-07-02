using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    /// <summary>
    /// Plugin for processing 'WebDeleteStudy' WorkQueue items.
    /// </summary>
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class TierMigrateStudyFactoryExtension  : IWorkQueueProcessorFactory
    {
        #region Constructors
        #endregion

        #region IWorkQueueProcessorFactory Members

        public  WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.MigrateStudy;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            return new TierMigrateItemProcessor();
        }

        #endregion
    }
}
