#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.ExtensionBrowser.PluginView
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
            get { return string.Format(SR.TreePluginViewExtensionPointsNode, ChildNodes.Count); }
        }

        public override string Details
        {
            get { return string.Format(SR.TreePluginViewExtensionPointsNodeDetails, GetDefaultDisplayName(_plugin)); }
        }
    }
}
