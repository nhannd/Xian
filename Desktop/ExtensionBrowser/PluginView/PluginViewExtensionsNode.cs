using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;


namespace ClearCanvas.Desktop.ExtensionBrowser.PluginView
{
    public class PluginViewExtensionsNode: BrowserNode
    {
        private PluginInfo _plugin;

        internal PluginViewExtensionsNode(PluginInfo plugin)
        {
            _plugin = plugin;
        }

        protected override void CreateChildNodes()
        {
            foreach (ExtensionInfo e in _plugin.Extensions)
            {
                AddChild(new PluginViewExtensionNode(e));
            }
        }

        public override string DisplayName
        {
            get { return string.Format(SR.TreePluginViewExtensionsNode, ChildNodes.Count); }
        }

        public override string Details
        {
            get { return string.Format(SR.TreePluginViewExtensionsNodeDetails, GetDefaultDisplayName(_plugin)); }
        }
    }
}
