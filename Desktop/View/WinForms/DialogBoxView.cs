using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IDialogBoxView"/>. 
    /// </summary>
    /// <remarks>
    /// This class may subclassed if customization is desired.  In this case, the <see cref="DesktopWindowView"/>
    /// class must also be subclassed in order to instantiate the subclass from 
    /// its <see cref="DesktopWindowView.CreateDialogBoxView"/> method.
    /// </remarks>
    public class DialogBoxView : DesktopObjectView, IDialogBoxView
    {
        private DialogBoxForm _form;
        private IWin32Window _owner;
        private bool _reallyClose;
        private DialogResult _result;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dialogBox"></param>
        /// <param name="owner"></param>
        protected internal DialogBoxView(DialogBox dialogBox, IWin32Window owner)
        {
            IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(dialogBox.Component.GetType());
            componentView.SetComponent((IApplicationComponent)dialogBox.Component);

            _form = new DialogBoxForm(dialogBox.Title, (Control)componentView.GuiElement);
            _form.FormClosing += new FormClosingEventHandler(_form_FormClosing);

            _owner = owner;
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public override void Open()
        {
            // do nothing
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public override void Show()
        {
            // do nothing
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public override void Hide()
        {
            // do nothing
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public override void Activate()
        {
            // do nothing
        }

        /// <summary>
        /// Sets the title of the dialog box.
        /// </summary>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
            _form.Text = title;
        }


        #region IDialogBoxView Members

        /// <summary>
        /// Runs the modal dialog.
        /// </summary>
        /// <returns></returns>
        public DialogBoxAction RunModal()
        {
            DialogResult result = _form.ShowDialog(_owner);
            switch (result)
            {
                case DialogResult.Cancel:
                    return DialogBoxAction.Cancel;
                case DialogResult.OK:
                    return DialogBoxAction.Ok;
                case DialogResult.No:
                    return DialogBoxAction.No;
                case DialogResult.Yes:
                    return DialogBoxAction.Yes;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Terminates the modal dialog.
        /// </summary>
        /// <param name="action"></param>
        public void EndModal(DialogBoxAction action)
        {
            switch (action)
            {
                case DialogBoxAction.Cancel:
                    _result = DialogResult.Cancel;
                    break;
                case DialogBoxAction.Ok:
                    _result = DialogResult.OK;
                    break;
                case DialogBoxAction.No:
                    _result = DialogResult.No;
                    break;
                case DialogBoxAction.Yes:
                    _result = DialogResult.Yes;
                    break;
            }
            _form.DialogResult = _result;
        }

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _reallyClose = true;
            }
            base.Dispose(disposing);
        }

        #endregion
    
        private void _form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // raise the close requested event
            // if this results in an actual close, the Dispose method will be called, setting the _reallyClose flag
            RaiseCloseRequested();
            e.Cancel = !_reallyClose;
        }

    }
}
