using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Common.MessageBoxExtensionPoint))]
    [GuiToolkit(GuiToolkitID.WinForms)]
	public class MessageBox : IMessageBox
	{
        /// <summary>
        /// Maps <see cref="ClearCanvas.Common.MessageBoxButtons"/> values to 
        /// <see cref="System.Windows.Forms.MessageBoxButtons"/> values.
        /// </summary>
        private static Dictionary<int, System.Windows.Forms.MessageBoxButtons> _buttonMap;
        private static Dictionary<DialogResult, int> _resultMap;

        static MessageBox() {

            _buttonMap = new Dictionary<int, System.Windows.Forms.MessageBoxButtons>();
            _buttonMap.Add((int)MessageBoxActions.Ok, MessageBoxButtons.OK);
            _buttonMap.Add((int)MessageBoxActions.OkCancel, MessageBoxButtons.OKCancel);
            _buttonMap.Add((int)MessageBoxActions.YesNo, MessageBoxButtons.YesNo);
            _buttonMap.Add((int)MessageBoxActions.YesNoCancel, MessageBoxButtons.YesNoCancel);

            _resultMap = new Dictionary<DialogResult, int>();
            _resultMap.Add(DialogResult.OK, (int)MessageBoxAction.Ok);
            _resultMap.Add(DialogResult.Cancel, (int)MessageBoxAction.Cancel);
            _resultMap.Add(DialogResult.Yes, (int)MessageBoxAction.Yes);
            _resultMap.Add(DialogResult.No, (int)MessageBoxAction.No);
        }

        public MessageBox()
        {
            // better not assume that SWF exists on a non-windows platform
            if (!Platform.IsWin32Platform)
                throw new NotSupportedException();
        }

		public void Show(string message)
		{
			System.Windows.Forms.MessageBox.Show(message, DesktopApplication.ApplicationName);
		}

        public MessageBoxAction Show(string message, ClearCanvas.Common.MessageBoxActions buttons)
        {
            DialogResult dr = System.Windows.Forms.MessageBox.Show(
                message, DesktopApplication.ApplicationName, _buttonMap[(int)buttons]);
            return (MessageBoxAction)_resultMap[dr];
        }

    }
}
