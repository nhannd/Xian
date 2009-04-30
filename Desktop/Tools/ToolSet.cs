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
        /// Constructs a toolset that is initially empty.
        /// </summary>
        public ToolSet()
        {
            _tools = new List<ITool>();
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
            :this()
        {
            AddTools(toolExtensionPoint, context);
        }

        /// <summary>
        /// Adds tools to the toolset based on the specified extension point and context.
        /// </summary>
        /// <remarks>
		/// The toolset will attempt to instantiate and initialize all 
		/// extensions of the specified tool extension point.
		/// </remarks>
        /// <param name="toolExtensionPoint">The tool extension point that provides the tools.</param>
        /// <param name="context">The tool context to pass to each tool.</param>
        private void AddTools(IExtensionPoint toolExtensionPoint, IToolContext context)
        {
            object[] tools = toolExtensionPoint.CreateExtensions();
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
