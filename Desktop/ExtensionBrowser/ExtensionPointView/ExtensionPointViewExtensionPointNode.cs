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
    public class ExtensionPointViewExtensionPointNode : BrowserNode
    {
        private ExtensionPointInfo _ep;

        internal ExtensionPointViewExtensionPointNode(ExtensionPointInfo ep)
        {
            _ep = ep;
        }

        protected override void CreateChildNodes()
        {
            // pass those extensions to a child node
            AddChild(new ExtensionPointViewExtensionsNode(_ep.ListExtensions()));
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
