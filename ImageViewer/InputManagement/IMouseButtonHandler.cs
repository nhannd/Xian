using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IMouseButtonHandler
	{
		bool Start(IMouseInformation mouseInformation);
		bool Track(IMouseInformation mouseInformation);
		bool Stop(IMouseInformation mouseInformation);
		void Cancel();

		bool SuppressContextMenu { get; }
		bool ConstrainToTile { get; }
	}
}
