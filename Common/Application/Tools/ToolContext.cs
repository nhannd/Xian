using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Application.Actions;

namespace ClearCanvas.Common.Application.Tools
{
    /// <summary>
    /// Provides methods to allow a tool to interact with its environment.
    /// </summary>
    public abstract class ToolContext
    {
        private List<ITool> _tools;
        private bool _initialized;

        private ActionModelRoot _menuModel;
        private ActionModelRoot _toolbarModel;

        private List<ToolViewProxy> _toolViews;

        private IExtensionPoint _toolExtensionPoint;



        /// <summary>
        /// Default constructor
        /// </summary>
        internal ToolContext(IExtensionPoint toolExtensionPoint)
        {
            _tools = new List<ITool>();
            _toolViews = new List<ToolViewProxy>();
            _toolExtensionPoint = toolExtensionPoint;
        }

        /// <summary>
        /// Used by a tool to register a view with the framework.
        /// </summary>
        /// <remarks>
        /// The tool should only call this method during its initialization (<see cref="ITool.Initialize"/>).
        /// In general, it is easier to use the <see cref="ToolViewAttribute"/> to declare the view,
        /// rather than creating and registering it at runtime.
        /// </remarks>
        /// <param name="viewProxy">The view to register</param>
        public void RegisterView(ToolViewProxy viewProxy)
        {
            _toolViews.Add(viewProxy);
        }

        /// <summary>
        /// Used by a tool to register an action with the framework.
        /// </summary>
        /// <remarks>
        /// The tool should only call this method during its initialization (<see cref="ITool.Initialize"/>).
        /// In general, it is easier to use the <see cref="ActionAttribute"/> subclasses to declare the action,
        /// rather than creating and registering it at runtime.
        /// </remarks>
        /// <param name="action"></param>
        public void RegisterAction(IAction action)
        {
            switch (action.Category)
            {
                case ActionCategory.MenuAction:
                    _menuModel.InsertAction(action);
                    break;
                case ActionCategory.ToolbarAction:
                    _toolbarModel.InsertAction(action);
                    break;
            }
        }

        internal ActionModelRoot MenuModel
        {
            get { return _menuModel; }
        }

        internal ActionModelRoot ToolbarModel
        {
            get { return _toolbarModel; }
        }

        internal ToolViewProxy[] ToolViews
        {
            get { return _toolViews.ToArray(); }
        }

        
        internal void Activate(bool active)
        {
            if (active && !_initialized)
            {
                // create the action models here (the tool may add actions to the models during its initialization)
                _menuModel = new ActionModelRoot(GetContextID() + ".menu");
                _toolbarModel = new ActionModelRoot(GetContextID() + ".toolbars");

                InitializeTools();
                BuildActionModels();

                _initialized = true;
            }
        }

        private void InitializeTools()
        {
            object[] tools = _toolExtensionPoint.CreateExtensions();
            foreach (ITool tool in tools)
            {
                tool.SetContext(this);
                _tools.Add(tool);

                tool.Initialize();
                ToolAttributeProcessor.Process(tool, this);
            }
        }

        private void BuildActionModels()
        {
            // process the action attributes
            foreach (ITool tool in _tools)
            {
                _menuModel.InsertActions(ActionAttributeProcessor.Process(tool, ActionCategory.MenuAction));
                _toolbarModel.InsertActions(ActionAttributeProcessor.Process(tool, ActionCategory.ToolbarAction));
            }

            // re-organize the models based on the stored models
            ActionModelStore store = new ActionModelStore("actionmodels.xml", false);
            _menuModel = store.Load(_menuModel.ModelID, _menuModel.GetActionsInOrder());
            _toolbarModel = store.Load(_toolbarModel.ModelID, _toolbarModel.GetActionsInOrder());

            // immediately update the stored models (because there is no ToolContext.Dispose() method)
            store.Save(_menuModel);
            store.Save(_toolbarModel);
            store.Flush();
        }

        private string GetContextID()
        {
            return _toolExtensionPoint.GetType().FullName;
        }
    }
}
