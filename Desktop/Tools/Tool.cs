using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="ITool"/>.  Tool classes should
    /// inherit this class rather than implement <see cref="ITool"/> directly.
    /// </summary>
	public abstract class Tool : ITool
	{
        private ToolContext _context;

        public void SetContext(ToolContext context)
        {
            _context = context;
        }

        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Provides a reference to the context in which this tool lives.
        /// </summary>
        /// <remarks>
        /// Do not call attempt to access this property in the constructor.
        /// It is valid only after the <see cref="SetContext"/> method has been called by
        /// the framework
        /// </remarks>
        protected ToolContext Context
        {
            get { return _context; }
        }
    }
}
