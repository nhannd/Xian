using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Application.Actions;

namespace ClearCanvas.Common.Application.Tools
{
    /// <summary>
    /// Undocumented - API subject to change
    /// </summary>
    public class ToolManager
    {
        private ToolContext _context;


        internal ToolManager(ToolContext context)
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
    }
}
