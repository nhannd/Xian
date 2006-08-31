using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Defines a tool.  Developers are encouraged to
    /// subclass <see cref="Tool"/> or one of its subclasses rather than implement this interface
    /// directly.
    /// </summary>
    public interface ITool : IDisposable
    {
        /// <summary>
        /// Called by the framework to set the tool context.
        /// </summary>
        void SetContext(IToolContext context);

        /// <summary>
        /// Called by the framework to allow the tool to initialize itself.  This method will
        /// be called after <see cref="SetContext"/> has been called, which guarantees that 
        /// the tool will have access to its context when this method is called.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets the set of actions that act on this tool.  This property should not be considered
        /// a dynamic property - that is, there is no point having this property return a different
        /// set of actions depending on the internal state of the tool.  The framework may choose to
        /// call this property only once during the lifetime of the tool.
        /// </summary>
        IActionSet Actions { get; }
    }
}
