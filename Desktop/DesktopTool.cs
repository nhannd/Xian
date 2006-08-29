using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    public abstract class DesktopTool : Tool
    {
        protected IDesktopToolContext Context
        {
            get { return (IDesktopToolContext)this.ContextBase; }
        }
    }
}
