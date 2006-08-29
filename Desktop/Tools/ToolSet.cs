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
        private List<ITool> _tools;

        public ToolSet()
        {
            _tools = new List<ITool>();
        }

        public ToolSet(IExtensionPoint toolExtensionPoint, IToolContext context)
            :this()
        {
            AddTools(toolExtensionPoint, context);
        }

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
                    Platform.Log(e, LogLevel.Error);
                }
            }
        }
    }
}
