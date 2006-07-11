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
        private IToolContext _context;
        private List<ITool> _tools;
        private bool _initialized;

        private ActionModelRoot _menuModel;
        private ActionModelRoot _toolbarModel;

        private List<ToolViewProxy> _toolViews;

        private IExtensionPoint _toolExtensionPoint;


        public ToolSet(IExtensionPoint toolExtensionPoint, IToolContext context)
        {
            _toolExtensionPoint = toolExtensionPoint;
            _context = context;

            _tools = new List<ITool>();
            _toolViews = new List<ToolViewProxy>();

            _initialized = false;
       }

        public ActionModelRoot MenuModel
        {
            get { return _menuModel; }
        }

        public ActionModelRoot ToolbarModel
        {
            get { return _toolbarModel; }
        }

        public ToolViewProxy[] ToolViews
        {
            get { return _toolViews.ToArray(); }
        }

        public void Activate(bool activate)
        {
            if (activate && !_initialized)
            {
                // create the action models here (the tool may add actions to the models during its initialization)
                _menuModel = new ActionModelRoot(GetContextID() + ".menu");
                _toolbarModel = new ActionModelRoot(GetContextID() + ".toolbars");

                InitializeTools();
                BuildActionModels();

                _initialized = true;
            }
        }

        internal void RegisterView(ToolViewProxy viewProxy)
        {
            _toolViews.Add(viewProxy);
        }

        private void InitializeTools()
        {
            object[] tools = _toolExtensionPoint.CreateExtensions();
            foreach (ITool tool in tools)
            {
                tool.SetContext(_context);
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
