using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using System.Drawing;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IContextMenuProvider
	{
		ActionModelNode GetContextMenuModel(Point point);
	}
}
