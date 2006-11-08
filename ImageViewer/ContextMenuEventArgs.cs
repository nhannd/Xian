using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer
{
	public class ContextMenuEventArgs : EventArgs
	{
		private ActionModelRoot _contextMenuModel;

		public ContextMenuEventArgs(ActionModelRoot contextMenuModel)
		{
			_contextMenuModel = contextMenuModel;
		}

		public ActionModelRoot ContextMenuModel
		{
			get { return _contextMenuModel; }
		}
	}
}
