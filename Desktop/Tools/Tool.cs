using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="ITool"/>.  Tool classes should
    /// inherit this class rather than implement <see cref="ITool"/> directly.
    /// </summary>
    public abstract class Tool<TContextInterface> : ToolBase
        where TContextInterface: IToolContext
	{

        /// <summary>
        /// Provides a typed reference to the context in which the tool is operating. Attempting to access this property
        /// before <see cref="SetContext"/> has been called (e.g in the constructor of this tool) will return null.
        /// </summary>
        protected TContextInterface Context
        {
            get { return (TContextInterface)this.ContextBase; }
        }
    }
}
