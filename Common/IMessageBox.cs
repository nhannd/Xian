using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
	[ClearCanvas.Common.ExtensionPoint()]
	public interface IMessageBox
	{
		void Show(string message);
	}
}
