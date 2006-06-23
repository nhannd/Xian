using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Undocumented - API subject to change
    /// </summary>
    public static class ToolAttributeProcessor
    {
        public static void Process(ITool tool, ToolContext context)
        {
            ToolBuilder builder = new ToolBuilder(context, tool);

            object[] attributes = tool.GetType().GetCustomAttributes(typeof(ToolAttribute), true);
            foreach (ToolAttribute a in attributes)
            {
                a.Apply(builder);
            }
        }
    }
}
