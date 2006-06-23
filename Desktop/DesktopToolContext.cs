using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for tools that are applicable to the desktop as a whole.
    /// </summary>
    [ExtensionPoint()]
    public class DesktopToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public class DesktopToolContext : ToolContext
    {
        public DesktopToolContext()
            : base(new DesktopToolExtensionPoint())
        {
        }
    }
}
