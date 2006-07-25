using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(ClearCanvas.Desktop.DialogBoxExtensionPoint))]
    [GuiToolkit(GuiToolkitID.WinForms)]
    public class DialogBox : IDialogBox
    {
        private DialogBoxForm _form;
        private DialogBoxAction _endAction;
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
            _form.ShowDialog();
            return _endAction;
        }

        public void EndModal(DialogBoxAction action)
        {
            // save this value to return it to the caller
            _endAction = action;

            // close the dialog - don't care about the DialogResult value because it is not used
            _form.DialogResult = DialogResult.OK;
        }

        #endregion
    }
}
