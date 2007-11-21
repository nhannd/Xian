using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Enumerated values indicating the result of WorkQueue item processing.
    /// </summary>
    public enum ProcessResultEnum
    { 
        FAILED,                 // The queue item status is set to failed
        SUCCESSFUL_PENDING,     // The queue item has been processed successfully and its status is set to pending.
        SUCCESSFUL_COMPLETED    // The queue item has been processed successfully and its status is set to completed.
        
    }

}
