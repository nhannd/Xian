using System;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    [ExtensionOf(typeof(TimeDialogViewExtensionPoint))]
    public class TimedDialogView : WinFormsView, ITimeDialogView
    {
        private TimedDialogForm _form;
        private TimedDialog _dialog;
        private bool _closed;

        public override object GuiElement
        {
            get { return _form; }
        }

        #region ITimeDialogView Members

        public void SetDialog(TimedDialog dialog)
        {
            if (_closed)
                throw new InvalidOperationException("This timed dialog view has been used.");

            _dialog = dialog;
        }

        public void Open()
        {
            if (_closed)
                throw new InvalidOperationException("This timed dialog view has been used.");

            _form = new TimedDialogForm(new TimedDialogControl(_dialog), _dialog.Title, _dialog.LingerTime);
            _form.CloseRequested += FormOnCloseRequested;
            _form.StartPosition = FormStartPosition.CenterParent;

            var owner = Form.ActiveForm;
            if (owner != null)
            {
                _form.Show(owner);
            }
            else
            {
                _form.Show();
            }

            _form.Activate();
        }

        public void Close()
        {
            if (_closed)
                return;

            _form.Dispose();
            _form = null;
            _closed = true;
        }

        #endregion

        private void FormOnCloseRequested(object sender, EventArgs eventArgs)
        {
            _dialog.Close();
        }
    }
}
