using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using Crownwood.DotNetMagic.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Abstract base class for all WinForms-based views.  Any class that implements a view using
    /// <see cref="System.Windows.Forms"/> as the underlying GUI toolkit should subclass this class.
    /// </summary>
    [GuiToolkit(ClearCanvas.Common.GuiToolkitID.WinForms)]
    public abstract class WinFormsView
    {
        protected WinFormsView()
        {
        }

        /// <summary>
        /// Returns <see cref="GuiToolkitID.WinForms"/>
        /// </summary>
        public string GuiToolkitID
        {
            get { return ClearCanvas.Common.GuiToolkitID.WinForms; }
        }

        /// <summary>
        /// Returns the <see cref="System.Windows.Forms.Control"/> that implements this view, allowing
        /// a parent view to insert the control as one of its children.
        /// </summary>
        public abstract object GuiElement
        {
            get;
        }
	}
}