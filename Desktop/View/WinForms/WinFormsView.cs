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
    /// <see cref="System.Windows.Forms"/> as the underlying GUI toolkit, and that intends to be compatible
    /// with <see cref="ClearCanvas.Workstation.View.WinForms.WorkstationView"/> must subclass this class.
    /// </summary>
    [GuiToolkit(GuiToolkitID.WinForms)]
    public abstract class WinFormsView
    {
        public WinFormsView()
        {
            if (!Platform.IsWin32Platform)
            {
                //TODO add a message here
                throw new NotSupportedException();
            }
            System.Windows.Forms.Application.EnableVisualStyles();
        }

        /// <summary>
        /// Returns <see cref="GuiToolkitID.WinForms"/>
        /// </summary>
        public GuiToolkitID GuiToolkitID
        {
            get { return GuiToolkitID.WinForms; }
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