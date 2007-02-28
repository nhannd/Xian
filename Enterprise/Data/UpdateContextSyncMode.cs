using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Data
{
    /// <summary>
    /// Specifies possible synchronization modes for a <see cref="IUpdateContext"/>
    /// </summary>
    public enum UpdateContextSyncMode
    {
        /// <summary>
        /// The context may write changes to the persistent store as necessary
        /// </summary>
        Flush,

        /// <summary>
        /// The context must hold all changes in memory
        /// </summary>
        Hold
    }
}
