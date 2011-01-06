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
			get { return string.Format(SR.FormatExtensionPointsDisplayName, Platform.PluginManager.ExtensionPoints.Count); }
        }

        public override string Details
        {
            get { return ""; }
        }
    }
}
