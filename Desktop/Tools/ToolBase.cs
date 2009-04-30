#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="ITool"/>.
    /// </summary>
    /// <remarks>
	/// Tool classes may inherit this class, but inheriting 
	/// from <see cref="Tool{TContextInterface}"/> is recommended.
	/// </remarks>
    public abstract class ToolBase : ITool
    {
        private IToolContext _context;
        private IActionSet _actions;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ToolBase()
		{
		}

        /// <summary>
        /// Provides an untyped reference to the context in which the tool is operating.
        /// </summary>
        /// <remarks>
		/// Attempting to access this property before <see cref="SetContext"/> 
		/// has been called (e.g in the constructor of this tool) will return null.
		/// </remarks>
        protected IToolContext ContextBase
        {
            get { return _context; }
        }

        #region ITool members

    	/// <summary>
    	/// Called by the framework to set the tool context.
    	/// </summary>
    	public void SetContext(IToolContext context)
        {
            _context = context;
        }

    	/// <summary>
    	/// Called by the framework to allow the tool to initialize itself.
    	/// </summary>
    	/// <remarks>
    	/// This method will be called after <see cref="SetContext"/> has been called, 
    	/// which guarantees that the tool will have access to its context when this method is called.
    	/// </remarks>
    	public virtual void Initialize()
        {
            // nothing to do
        }

    	/// <summary>
    	/// Gets the set of actions that act on this tool.
    	/// </summary>
    	/// <remarks>
    	/// <see cref="ITool.Actions"/> mentions that this property should not be considered dynamic.
    	/// This implementation assumes that the actions are <b>not</b> dynamic by lazily initializing
    	/// the actions and storing them.  If you wish to return actions dynamically, you must override
    	/// this property.
    	/// </remarks>
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

		/// <summary>
		/// Disposes of this object; override this method to do any necessary cleanup.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context = null;
			}
		}
		
		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
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
