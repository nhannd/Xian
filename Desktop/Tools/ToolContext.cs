using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Base class for all tool contexts.  Implementations of <see cref="IToolContext"/> are encouraged to
    /// inherit this class rather than implement the interface directly.
    /// </summary>
    public abstract class ToolContext : IToolContext
    {
    }
}
