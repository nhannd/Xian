using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// A convenience class that serves as a base class for tools that extend
    /// <see cref="DesktopToolExtensionPoint"/>.  These tools should inherit
    /// this base class.
    /// </summary>
    public abstract class DesktopTool : Tool
    {
        /// <summary>
        /// Provides access to the associated <see cref="IDesktopToolContext"/>
        /// </summary>
        protected IDesktopToolContext Context
        {
            get { return (IDesktopToolContext)this.ContextBase; }
        }
    }
}
