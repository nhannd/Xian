using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.ExtensionBrowser.ExtensionPointView
{
    public class ExtensionPointViewExtensionPointNode : BrowserNode
    {
        private ExtensionPointInfo _ep;

        internal ExtensionPointViewExtensionPointNode(ExtensionPointInfo ep)
        {
            _ep = ep;
        }

        protected override void CreateChildNodes()
        {
            // determine the set of extensions of this extension point
            List<ExtensionInfo> extensions = new List<ExtensionInfo>();
            foreach (ExtensionInfo e in Platform.PluginManager.Extensions)
            {
                if (e.ExtensionPointClass == _ep.ExtensionPointClass)
                {
                    extensions.Add(e);
                }
            }

            // pass those extensions to a child node
            AddChild(new ExtensionPointViewExtensionsNode(extensions.ToArray()));
        }

        public override string DisplayName
        {
            get { return GetDefaultDisplayName(_ep); }
        }

        public override string Details
        {
            get { return GetDefaultDetails(_ep); }
        }
    }
}
