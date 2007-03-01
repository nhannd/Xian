using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Used by <see cref="IUpdateContext.Lock"/> to indicate whether an entity should be considered clean or dirty.
    /// </summary>
    public enum DirtyState
    {
        /// <summary>
        /// Treat the entity as clean
        /// </summary>
        Clean,

        /// <summary>
        /// Treat the entity as dirty
        /// </summary>
        Dirty,

        /// <summary>
        /// Treat the entity as new (unsaved)
        /// </summary>
        New
    }
}
