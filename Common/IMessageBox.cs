using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    [Flags]
    public enum MessageBoxResult
    {
        Ok      = 0x0001,
        Cancel  = 0x0002,
        Yes     = 0x0004,
        No      = 0x0008,
    }

    public enum MessageBoxButtons
    {
        Ok = MessageBoxResult.Ok,
        OkCancel = MessageBoxResult.Ok | MessageBoxResult.Cancel,
        YesNo = MessageBoxResult.Yes | MessageBoxResult.No,
        YesNoCancel = MessageBoxResult.Yes | MessageBoxResult.No | MessageBoxResult.Cancel
    }

	public interface IMessageBox
	{
		void Show(string message);
        MessageBoxResult Show(string message, MessageBoxButtons buttons);
	}
}
