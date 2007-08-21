using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Queue
{
    public interface IWorkQueueItemProcessor : IDisposable
    {
        void Process(WorkQueue item);
    }
}
