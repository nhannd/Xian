using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="ITool"/>.  Tool classes should
    /// inherit this class rather than implement <see cref="ITool"/> directly.
    /// </summary>
	public abstract class Tool : ITool
	{
        private IToolContext _context;
        private IActionSet _actions;

        public Tool()
        {
        }

        ~Tool()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            _context = null;
        }

        public void SetContext(IToolContext context)
        {
            _context = context;
        }

        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Provides a reference to the context in which the tool is operating.
        /// May return null if there is no current context.
        /// </summary>
        /// <remarks>
        /// Subclasses will probably want to create a property that wraps this property
        /// but casts the return value to the expected interface.
        /// </remarks>
        protected IToolContext ContextBase
        {
            get { return _context; }
        }

        public virtual IActionSet Actions
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new ActionSet(ActionAttributeProcessor.Process(this));
                }
                return _actions;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
