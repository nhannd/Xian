using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Undocumented - API subject to change
    /// </summary>
    public class ToolSet : IToolSet
    {
        private ToolContext _context;


        public ToolSet(ToolContext context)
        {
            _context = context;
        }

        public ActionModelRoot MenuModel
        {
            get { return _context.MenuModel; }
        }

        public ActionModelRoot ToolbarModel
        {
            get { return _context.ToolbarModel; }
        }

        public ToolViewProxy[] ToolViews
        {
            get { return _context.ToolViews; }
        }

        public void Activate(bool activate)
        {
            _context.Activate(activate);
        }
    }
}
