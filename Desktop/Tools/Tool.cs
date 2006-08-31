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

        /// <summary>
        /// Constructor
        /// </summary>
        public Tool()
        {
        }

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> pattern
        /// </summary>
        /// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context = null;
            }
        }

        /// <summary>
        /// Provides a reference to the context in which the tool is operating. Attempting to access this property
        /// before <see cref="SetContext"/> has been called (e.g in the constructor of this tool) will return null.
        /// </summary>
        /// <remarks>
        /// Subclasses will probably want to create a property that wraps this property
        /// but casts the return value to the expected interface.
        /// </remarks>
        protected IToolContext ContextBase
        {
            get { return _context; }
        }

        #region ITool members

        public void SetContext(IToolContext context)
        {
            _context = context;
        }

        public virtual void Initialize()
        {
            // nothing to do
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

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                // shouldn't throw anything from inside Dispose()
                Platform.Log(e);
            }
        }

        #endregion
    }
}
