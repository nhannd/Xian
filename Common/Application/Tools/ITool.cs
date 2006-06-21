using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Tools
{
    /// <summary>
    /// Used by the framework to initialize a tool.  All tools must implement this interface, but
    /// developers are encouraged to subclass <see cref="Tool"/> rather than implement this interface
    /// directly.
    /// </summary>
    public interface ITool
    {
        /// <summary>
        /// Called by the framework when the tool is first loaded, to set its context.
        /// </summary>
        /// <param name="context">A context which provides the tool with a sense of its environment.</param>
        void SetContext(ToolContext context);

        /// <summary>
        /// Called by the framework to allow the tool to initialize itself.  This method will
        /// always be called after <see cref="SetContext"/>, because the tool may require access
        /// to the context in order to properly initialize itself.
        /// </summary>
        void Initialize();
    }
}
