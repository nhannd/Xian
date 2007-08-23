using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Queue
{
    /// <summary>
    /// Plugin for WorkQueue item processors.
    /// </summary>
    [ExtensionPoint()]
    public class WorkQueueFactoryExtensionPoint : ExtensionPoint<IWorkQueueProcessorFactory>
    {
    }
}
