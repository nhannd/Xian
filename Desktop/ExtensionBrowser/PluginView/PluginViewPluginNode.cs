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
