using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Defines an interface of a component which can be attached to a <see cref="IWorkQueueItemProcessor"/>.
    /// </summary>
    /// <typeparam name="TProcessor">The work queue item processor class which implements <seealso cref="IWorkQueueItemProcessor"/>.</typeparam>
    public interface IWorkQueueProcessorListener<TProcessor> where TProcessor:IWorkQueueItemProcessor
    {
        /// <summary>
        /// Called to initialize the component.
        /// </summary>
        /// <param name="processor">The work queue item processor.</param>
        void Initialize(TProcessor processor);

    }
}
