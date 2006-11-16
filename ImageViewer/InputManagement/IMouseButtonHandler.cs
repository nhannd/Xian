using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IMouseButtonHandler
	{
		bool Start(MouseInformation pointerInformation);
		bool Track(MouseInformation pointerInformation);
		bool Stop(MouseInformation pointerInformation);
	}
}
