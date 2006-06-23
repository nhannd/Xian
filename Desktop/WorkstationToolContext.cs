using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Application.Tools;

namespace ClearCanvas.Common.Application
{
    /// <summary>
    /// Defines an extension point for tools that are applicable to the workstation as a whole.
    /// </summary>
    [ExtensionPoint()]
    public class WorkstationToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public class WorkstationToolContext : ToolContext
    {
        public WorkstationToolContext()
            : base(new WorkstationToolExtensionPoint())
        {
        }
    }
}
