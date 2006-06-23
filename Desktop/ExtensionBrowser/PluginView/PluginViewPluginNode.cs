using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Workstation.ExtensionBrowser;

namespace ClearCanvas.Workstation.ExtensionBrowser.PluginView
{
    public class PluginViewPluginNode : BrowserNode
    {
        private PluginInfo _plugin;

        internal PluginViewPluginNode(PluginInfo plugin)
        {
            _plugin = plugin;
        }

        public override string DisplayName
        {
            get
            {
                return GetDefaultDisplayName(_plugin);
            }
        }

        public override string Details
        {
            get { return ""; }
        }

        protected override void CreateChildNodes()
        {
            AddChild(new PluginViewExtensionPointsNode(_plugin));
            AddChild(new PluginViewExtensionsNode(_plugin));
        }
    }
}
