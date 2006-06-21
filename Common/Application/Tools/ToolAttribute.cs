using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Tools
{
    /// <summary>
    /// Undocumented - API subject to change
    /// </summary>
    public abstract class ToolAttribute : Attribute
    {
        public abstract void Apply(ToolBuilder builder);
    }
}
