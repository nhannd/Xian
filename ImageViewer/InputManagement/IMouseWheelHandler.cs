using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IMouseWheelHandler
	{
		void Activate(int wheelDelta);
	}
}
