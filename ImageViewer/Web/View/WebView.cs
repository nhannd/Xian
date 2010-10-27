#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Web.View
{
     /// <summary>
    /// Abstract base class for all Web-based views.  Any class that implements a view using
    /// a Web View as the underlying GUI toolkit should subclass this class.
    /// </summary>
    [GuiToolkit(ClearCanvas.Common.GuiToolkitID.Web)]
    public abstract class WebView
    {
         /// <summary>
        /// Gets the toolkit ID, which is always <see cref="ClearCanvas.Common.GuiToolkitID.Web"/>.
        /// </summary>
        public string GuiToolkitID
        {
            get { return ClearCanvas.Common.GuiToolkitID.Web; }
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public object GuiElement
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }
}
