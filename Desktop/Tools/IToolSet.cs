using System;

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Defines the external interface to a tool set, which manages a set of tools
    /// </summary>
    public interface IToolSet : IDisposable
    {
        /// <summary>
        /// Gets the tools contained in this tool set
        /// </summary>
        ITool[] Tools { get; }
        
        /// <summary>
        /// Returns the union of all actions defined by all tools in this tool set
        /// </summary>
        IActionSet Actions { get; }
    }
}
