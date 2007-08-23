using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Queue
{
    /// <summary>
    /// Interface for factory to create instances of <see cref="IWorkQueueItemProcessor"/> for processing WorkQueue items.
    /// </summary>
    public interface IWorkQueueProcessorFactory
    {
        TypeEnum GetWorkQueueType();

        IWorkQueueItemProcessor GetItemProcessor();
    }
}
