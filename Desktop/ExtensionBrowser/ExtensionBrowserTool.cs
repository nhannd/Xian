using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Application.Tools;
using ClearCanvas.Common.Application.Actions;
using ClearCanvas.Workstation.Model;
using ClearCanvas.Workstation.ExtensionBrowser.PluginView;
using ClearCanvas.Workstation.ExtensionBrowser.ExtensionPointView;

namespace ClearCanvas.Workstation.ExtensionBrowser
{
    [ExtensionPoint()]
    public class ExtensionBrowserViewExtensionPoint : ExtensionPoint<IToolView>
    {
    }


    [MenuAction("show", "MenuFile/MenuExtensionBrowser", Flags=ClickActionFlags.CheckAction)]
    [ClickHandler("show", "ShowHide")]
    [CheckedStateObserver("show", "IsViewActive", "ViewActivationChanged")]

    [ToolView(typeof(ExtensionBrowserViewExtensionPoint), "TitleExtensionBrowser", ToolViewDisplayHint.DockLeft, "IsViewActive", "ViewActivationChanged")]

    /// <summary>
    /// Summary description for ExtensionBrowserTool.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Common.Application.WorkstationToolExtensionPoint))]
    public class ExtensionBrowserTool : Tool
	{
        private PluginViewRootNode _pluginViewTree;
        private ExtensionPointViewRootNode _extPtViewTree;

        private bool _showView;
        private event EventHandler _viewActivationChanged;

        public ExtensionBrowserTool()
		{
            // create the browser trees here - this will not incur any real cost,
            // because each level of the browser tree does not load it's children until requested
            _pluginViewTree = new PluginViewRootNode();
            _extPtViewTree = new ExtensionPointViewRootNode();
        }

        public PluginViewRootNode PluginTree
        {
            get { return _pluginViewTree; }
        }

        public ExtensionPointViewRootNode ExtensionPointTree
        {
            get { return _extPtViewTree; }
        }

        public void ShowHide()
        {
            this.IsViewActive = !this.IsViewActive;
        }

        public bool IsViewActive
        {
            get { return _showView; }
            set
            {
                if (_showView != value)
                {
                    _showView = value;
                    EventsHelper.Fire(_viewActivationChanged, this, new EventArgs());
                }
            }
        }

        public event EventHandler ViewActivationChanged
        {
            add { _viewActivationChanged += value; }
            remove { _viewActivationChanged -= value; }
        }
    }
}
