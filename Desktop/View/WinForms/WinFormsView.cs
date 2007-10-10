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