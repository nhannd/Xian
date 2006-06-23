using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Workstation.ExtensionBrowser;

namespace ClearCanvas.Workstation.ExtensionBrowser.PluginView
{
    public class PluginViewExtensionPointsNode : BrowserNode
    {
        private PluginInfo _plugin;

        internal PluginViewExtensionPointsNode(PluginInfo plugin)
        {
            _plugin = plugin;
        }

        protected override void CreateChildNodes()
        {
            foreach (ExtensionPointInfo ep in _plugin.ExtensionPoints)
            {
                AddChild(new PluginViewExtensionPointNode(ep));
            }
        }

        public override string DisplayName
        {
            get { return string.Format("Extension Points ({0})", ChildNodes.Count); }
        }

        public override string Details
        {
            get { return string.Format("Extension Points declared in plugin {0}", GetDefaultDisplayName(_plugin)); }
        }
    }
}
