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
    public class ExtensionPointViewExtensionsNode : BrowserNode
    {
        private IList<ExtensionInfo> _extensions;

        internal ExtensionPointViewExtensionsNode(IList<ExtensionInfo> extensions)
        {
            _extensions = extensions;
        }

        protected override void CreateChildNodes()
        {
            foreach (ExtensionInfo e in _extensions)
            {
                AddChild(new ExtensionPointViewExtensionNode(e));
            }
        }

        public override string DisplayName
        {
            get { return string.Format(SR.TreeExtensionPointViewExtensionsNode, ChildNodes.Count); }
        }

        public override string Details
        {
            get { return ""; }
        }
    }
}
