using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Queue
{
    [ExtensionPoint()]
    public class WorkQueueFactoryExtensionPoint : ExtensionPoint<IWorkQueueProcessorFactory>
    {
    }
}
