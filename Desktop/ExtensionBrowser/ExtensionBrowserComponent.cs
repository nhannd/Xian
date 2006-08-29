using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

using ClearCanvas.Desktop.ExtensionBrowser.PluginView;
using ClearCanvas.Desktop.ExtensionBrowser.ExtensionPointView;

namespace ClearCanvas.Desktop.ExtensionBrowser
{
    /// <summary>
    /// Defines an extension point for views onto the <see cref="ExtensionBrowserComponent"/>
    /// </summary>
    [ExtensionPoint()]
    public class ExtensionBrowserComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// Component that displays the set of installed extensions in a tree view.
    /// </summary>
    [AssociateView(typeof(ExtensionBrowserComponentViewExtensionPoint))]
    public class ExtensionBrowserComponent : ApplicationComponent
    {
        private PluginViewRootNode _pluginViewTree;
        private ExtensionPointViewRootNode _extPtViewTree;


        public override void Start()
        {
            base.Start();

            // create the browser trees here - this will not incur any real cost,
            // because each level of the browser tree does not load it's children until requested
            _pluginViewTree = new PluginViewRootNode();
            _extPtViewTree = new ExtensionPointViewRootNode();
        }

        /// <summary>
        /// Gets a tree model that represents a "plugin-centric" view of the extensions
        /// </summary>
        public PluginViewRootNode PluginTree
        {
            get { return _pluginViewTree; }
        }

        /// <summary>
        /// Gets a tree model that represents an "extension point centric" view of the extensions
        /// </summary>
        public ExtensionPointViewRootNode ExtensionPointTree
        {
            get { return _extPtViewTree; }
        }
    }
}
