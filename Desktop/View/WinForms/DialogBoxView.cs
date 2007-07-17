using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class DialogBoxView : DesktopObjectView, IDialogBoxView
    {
        private DialogBoxForm _form;
        private IWin32Window _owner;
        private bool _reallyClose;
        private DialogResult _result;

        protected internal DialogBoxView(DialogBox dialogBox, IWin32Window owner)
        {
            IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(dialogBox.Component.GetType());
            componentView.SetComponent((IApplicationComponent)dialogBox.Component);

            _form = new DialogBoxForm(dialogBox.Title, (Control)componentView.GuiElement);
            _form.FormClosing += new FormClosingEventHandler(_form_FormClosing);

            _owner = owner;
        }

        public override void Open()
        {
            // do nothing
        }

        public override void Show()
        {
            // do nothing
        }

        public override void Hide()
        {
            // do nothing
        }

        public override void Activate()
        {
            // do nothing
        }

        public override void SetTitle(string title)
        {
            _form.Text = title;
        }


        #region IDialogBoxView Members

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
