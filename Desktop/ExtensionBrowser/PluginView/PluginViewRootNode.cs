using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Workstation.ExtensionBrowser;


namespace ClearCanvas.Workstation.ExtensionBrowser.PluginView
{
    public class PluginViewRootNode : BrowserNode
    {
        protected override void CreateChildNodes()
        {
            foreach (PluginInfo plugin in Platform.PluginManager.Plugins)
            {
                AddChild(new PluginViewPluginNode(plugin));
            }
        }

        public override string DisplayName
        {
            get { return string.Format("Plugins ({0})", ChildNodes.Count); }
        }

        public override string Details
        {
            get { return ""; }
        }
    }
}
