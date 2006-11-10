using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Used by <see cref="IUpdateContext.Lock"/> to indicate whether an entity should be considered clean or dirty.
    /// </summary>
    public enum DirtyState
    {
        Clean,
        Dirty
    }
}
