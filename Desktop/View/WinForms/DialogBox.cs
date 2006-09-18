using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(ClearCanvas.Desktop.DialogBoxExtensionPoint))]
    [GuiToolkit(GuiToolkitID.WinForms)]
    public class DialogBox : IDialogBox
    {
        private DialogBoxForm _form;
        private event EventHandler<ClosingEventArgs> _dialogClosing;

        #region IDialogBox Members

        public event EventHandler<ClosingEventArgs> DialogClosing
        {
            add { _dialogClosing += value; }
            remove { _dialogClosing -= value; }
        }


        public void Initialize(string title, IView view)
        {
            _form = new DialogBoxForm(title, view);
            //_form.Owner = (Form)DesktopApplication.View.GuiElement;
            _form.FormClosing += new FormClosingEventHandler(_form_FormClosing);
        }

        private void _form_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClosingEventArgs args = new ClosingEventArgs();
            EventsHelper.Fire(_dialogClosing, this, args);
            e.Cancel = args.Cancel;
        }

        public DialogBoxAction RunModal()
        {
            DialogResult result = _form.ShowDialog();
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
                    _form.DialogResult = DialogResult.Cancel;
                    break;
                case DialogBoxAction.Ok:
                    _form.DialogResult = DialogResult.OK;
                    break;
                case DialogBoxAction.No:
                    _form.DialogResult = DialogResult.No;
                    break;
                case DialogBoxAction.Yes:
                    _form.DialogResult = DialogResult.Yes;
                    break;
            }
        }

        #endregion
    }
}
