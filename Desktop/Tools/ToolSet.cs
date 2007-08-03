using System;
using System.Collections.Generic;
using System.Text;

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
        /// Constructs a toolset that is initially empty
        /// </summary>
        public ToolSet()
        {
            _tools = new List<ITool>();
        }

        /// <summary>
        /// Constructs a toolset based on the specified extension point and context. The toolset will
        /// attempt to instantiate and initialize all extensions of the specified tool extension point.
        /// </summary>
        /// <param name="toolExtensionPoint">The tool extension point that provides the tools</param>
        /// <param name="context">The tool context to pass to each tool</param>
        public ToolSet(IExtensionPoint toolExtensionPoint, IToolContext context)
            :this()
        {
            AddTools(toolExtensionPoint, context);
        }

        /// <summary>
        /// Adds tools to the toolset based on the specified extension point and context. The toolset will
        /// attempt to instantiate and initialize all extensions of the specified tool extension point.
        /// </summary>
        /// <param name="toolExtensionPoint">The tool extension point that provides the tools</param>
        /// <param name="context">The tool context to pass to each tool</param>
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
        /// Implementation of the <see cref="IDisposable"/> pattern
        /// </summary>
        /// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
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

        public ITool[] Tools
        {
            get { return _tools.ToArray(); }
        }

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
