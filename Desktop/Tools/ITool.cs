using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Used by the framework to initialize a tool.  All tools must implement this interface, but
    /// developers are encouraged to subclass <see cref="Tool"/> rather than implement this interface
    /// directly.
    /// </summary>
    public interface ITool : IDisposable
    {
        /// <summary>
        /// Called by the framework to set the tool's context.
        /// </summary>
        void SetContext(IToolContext context);

        /// <summary>
        /// Called by the framework to allow the tool to initialize itself.  This method will
        /// be called after the tool has been created but
        /// </summary>
        /// <param name="context">The context</param>
        void Initialize();

        /// <summary>
        /// Gets the set of actions that act on this tool.
        /// </summary>
        IActionSet Actions { get; }
    }
}
