using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.ExtensionBrowser.PluginView
{
    public class PluginViewExtensionNode : BrowserNode
    {
        private ExtensionInfo _ext;

        internal PluginViewExtensionNode(ExtensionInfo ext)
        {
            _ext = ext;
        }

        protected override void CreateChildNodes()
        {
            // no child nodes
        }

        public override string DisplayName
        {
            get { return GetDefaultDisplayName(_ext); }
        }

        public override string Details
        {
            get { return GetDefaultDetails(_ext); }
        }
    }
}
