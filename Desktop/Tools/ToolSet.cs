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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Default implementation of <see cref="IToolSet"/>.
    /// </summary>
    public class ToolSet : IToolSet
    {
        private List<ITool> _tools;

        /// <summary>
        /// This contructs a tool set containing the specified tools.  The <see cref="IToolContext"/>
        /// is set on each tool and each tool's Initialize method is called.
        /// </summary>
        /// <param name="context">The tool context to pass to each tool.</param>
        /// <param name="tools">A set of tools to group in this ToolSet and be initialized and 
        /// set with the same tool context.</param>
        public ToolSet(IEnumerable tools, IToolContext context)
        {
            _tools = new List<ITool>();

            foreach (ITool tool in tools)
            {
                try
                {
                    tool.SetContext(context);
                    tool.Initialize();
                    _tools.Add(tool);
                }
                catch (Exception e)
                {
                    // a tool failed to initialize - log and continue
                    // (this tool will not be included in the set)
                    Platform.Log(LogLevel.Error, e);
                }
            }
        }

        /// <summary>
        /// Constructs a toolset based on the specified extension point and context.
        /// </summary>
        /// <remarks>
		/// The toolset will attempt to instantiate and initialize all 
		/// extensions of the specified tool extension point.
		/// </remarks>
        /// <param name="toolExtensionPoint">The tool extension point that provides the tools.</param>
        /// <param name="context">The tool context to pass to each tool.</param>
        public ToolSet(IExtensionPoint toolExtensionPoint, IToolContext context)
            :this(toolExtensionPoint, context, null)
        {
        }

        /// <summary>
        /// Constructs a toolset based on the specified extension point and context.
        /// </summary>
        /// <remarks>
        /// The toolset will attempt to instantiate and initialize all 
        /// extensions of the specified tool extension point that pass the 
        /// specified filter.
        /// </remarks>
        /// <param name="toolExtensionPoint">The tool extension point that provides the tools.</param>
        /// <param name="context">The tool context to pass to each tool.</param>
        /// <param name="filter">Only tools that match the specified extension filter are loaded into the 
        /// tool set.  If null, all tools extending the extension point are loaded.</param>
        public ToolSet(IExtensionPoint toolExtensionPoint, IToolContext context, ExtensionFilter filter)
            :this(toolExtensionPoint.CreateExtensions(filter), context)
        {
        }

        /// <summary>
        /// Disposes of all the <see cref="ITool"/>s in the tool set.
        /// </summary>
        /// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (ITool tool in _tools)
                {
                    try
                    {
                        tool.Dispose();
                    }
                    catch (Exception e)
                    {
                        // log and continue disposing of other tools
                        Platform.Log(LogLevel.Error, e);
                    }
                }
            }
        }


        #region IToolSet members

    	/// <summary>
    	/// Gets the tools contained in this tool set.
    	/// </summary>
    	public ITool[] Tools
        {
            get { return _tools.ToArray(); }
        }

    	/// <summary>
    	/// Returns the union of all actions defined by all tools in this tool set.
    	/// </summary>
    	public IActionSet Actions
        {
            get
            {
                List<IAction> actionList = new List<IAction>();
                foreach (ITool tool in _tools)
                {
                    actionList.AddRange(tool.Actions);
                }
                return new ActionSet(actionList);
            }
        }

        #endregion

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
