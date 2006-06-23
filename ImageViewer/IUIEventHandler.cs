using System;

namespace ClearCanvas.Workstation.Model
{
	public interface IUIEventHandler 
	{
		bool OnMouseMove(XMouseEventArgs e);

		bool OnMouseDown(XMouseEventArgs e);

		bool OnMouseUp(XMouseEventArgs e);

		bool OnMouseWheel(XMouseEventArgs e);

		bool OnKeyDown(XKeyEventArgs e);

		bool OnKeyUp(XKeyEventArgs e);
	}
}


