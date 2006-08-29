using System;

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    public interface IToolSet
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
