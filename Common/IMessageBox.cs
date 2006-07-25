using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    [Flags]
    public enum DialogBoxAction
    {
        Ok      = 0x0001,
        Cancel  = 0x0002,
        Yes     = 0x0004,
        No      = 0x0008,
    }

    public enum MessageBoxActions
    {
        Ok = DialogBoxAction.Ok,
        OkCancel = DialogBoxAction.Ok | DialogBoxAction.Cancel,
        YesNo = DialogBoxAction.Yes | DialogBoxAction.No,
        YesNoCancel = DialogBoxAction.Yes | DialogBoxAction.No | DialogBoxAction.Cancel
    }

	public interface IMessageBox
	{
		void Show(string message);
        DialogBoxAction Show(string message, MessageBoxActions buttons);
	}
}
