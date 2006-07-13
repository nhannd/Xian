using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    [Flags]
    public enum MessageBoxAction
    {
        Ok      = 0x0001,
        Cancel  = 0x0002,
        Yes     = 0x0004,
        No      = 0x0008,
    }

    public enum MessageBoxActions
    {
        Ok = MessageBoxAction.Ok,
        OkCancel = MessageBoxAction.Ok | MessageBoxAction.Cancel,
        YesNo = MessageBoxAction.Yes | MessageBoxAction.No,
        YesNoCancel = MessageBoxAction.Yes | MessageBoxAction.No | MessageBoxAction.Cancel
    }

	public interface IMessageBox
	{
		void Show(string message);
        MessageBoxAction Show(string message, MessageBoxActions buttons);
	}
}
