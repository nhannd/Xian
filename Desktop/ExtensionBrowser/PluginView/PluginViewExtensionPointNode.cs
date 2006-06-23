using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.ExtensionBrowser.PluginView
{
    public class PluginViewExtensionPointNode : BrowserNode
    {
        private ExtensionPointInfo _ep;

        internal PluginViewExtensionPointNode(ExtensionPointInfo ep)
        {
            _ep = ep;
        }

        protected override void CreateChildNodes()
        {
            // no child nodes
        }

        public override string DisplayName
        {
            get
            {
                return GetDefaultDisplayName(_ep);
            }
        }

        public override string Details
        {
            get 
            {
                return GetDefaultDetails(_ep);
            }
        }
    }
}
