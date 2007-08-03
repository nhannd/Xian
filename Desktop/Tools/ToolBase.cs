using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="ITool"/>.  Tool classes may inherit
    /// this class, but inheriting from <see cref="Tool"/> is recommended.
    /// </summary>
    public abstract class ToolBase : ITool
    {
        private IToolContext _context;
        private IActionSet _actions;

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
        /// Provides an untyped reference to the context in which the tool is operating. Attempting to access this property
        /// before <see cref="SetContext"/> has been called (e.g in the constructor of this tool) will return null.
        /// </summary>
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
                Platform.Log(LogLevel.Error, e);
            }
        }

        #endregion
    }
}
