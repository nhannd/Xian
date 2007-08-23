using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Queue
{
    /// <summary>
    /// Interface for processors of WorkQueue items
    /// </summary>
    public interface IWorkQueueItemProcessor : IDisposable
    {
        void Process(WorkQueue item);
    }
}
