#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
            get { return string.Format(SR.TreePluginViewRootNode, ChildNodes.Count); }
        }

        public override string Details
        {
            get { return ""; }
        }
    }
}
