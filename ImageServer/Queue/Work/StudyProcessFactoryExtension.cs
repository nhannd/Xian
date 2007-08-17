using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Queue;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Queue.Work
{
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class StudyProcessFactoryExtension : IWorkQueueProcessorFactory
    {
        #region Private Members
        #endregion

        #region Constructors
        public StudyProcessFactoryExtension()
        { }
        #endregion



        #region IWorkQueueProcessorFactory Members

        public TypeEnum GetWorkQueueType()
        {
            return TypeEnum.GetEnum("StudyProcess");
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            return new StudyProcessItemProcessor();
        }

        #endregion
    }
}
