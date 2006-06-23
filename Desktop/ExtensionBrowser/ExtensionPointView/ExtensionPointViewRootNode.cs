using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.ExtensionBrowser.ExtensionPointView
{
    public class ExtensionPointViewRootNode : BrowserNode
    {
        protected override void CreateChildNodes()
        {
            foreach (ExtensionPointInfo ep in Platform.PluginManager.ExtensionPoints)
            {
                AddChild(new ExtensionPointViewExtensionPointNode(ep));
            }
        }

        public override string DisplayName
        {
            get { return string.Format("Extension Points ({0})", Platform.PluginManager.ExtensionPoints.Length); }
        }

        public override string Details
        {
            get { return ""; }
        }
    }
}
