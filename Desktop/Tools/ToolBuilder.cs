using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Undocumented - API subject to change
    /// </summary>
    public class ToolBuilder
    {
        private ToolContext _context;
        private ITool _tool;

        internal ToolBuilder(ToolContext context, ITool tool)
        {
            _context = context;
            _tool = tool;
        }

        public void Apply(ToolViewAttribute a)
        {
            IExtensionPoint xp = (IExtensionPoint)Activator.CreateInstance(a.ViewExtensionPoint);

            DynamicObservablePropertyBinding<bool> viewActivePropertyBinding =
                new DynamicObservablePropertyBinding<bool>(_tool, a.ActivatedPropertyName, a.ActivatedChangeEventName);

            ResourceResolver resolver = new ResourceResolver(new Assembly[] { _tool.GetType().Assembly });
            string title = resolver.Resolve(a.Title);

            _context.RegisterView(new ToolViewProxy(_tool, xp, title, a.DisplayHint, viewActivePropertyBinding));
        }
    }
}
