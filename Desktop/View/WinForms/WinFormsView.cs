#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Abstract base class for all WinForms-based views.  Any class that implements a view using
    /// WinForms as the underlying GUI toolkit should subclass this class.
    /// </summary>
    [GuiToolkit(ClearCanvas.Common.GuiToolkitID.WinForms)]
    public abstract class WinFormsView
    {
        protected WinFormsView()
        {
        }

        /// <summary>
        /// Gets the toolkit ID, which is always <see cref="ClearCanvas.Common.GuiToolkitID.WinForms"/>.
        /// </summary>
        public string GuiToolkitID
        {
            get { return ClearCanvas.Common.GuiToolkitID.WinForms; }
        }

        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.Control"/> that implements this view, allowing
        /// a parent view to insert the control as one of its children.
        /// </summary>
        public abstract object GuiElement
        {
            get;
        }
	}
}