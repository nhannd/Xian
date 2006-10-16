using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layers
{
	public class GraphicFocusedEventArgs : EventArgs
	{
		private Graphic _focusedGraphic;

		public GraphicFocusedEventArgs(
			Graphic focusedGraphic)
		{
			Platform.CheckForNullReference(focusedGraphic, "focusedGraphic");
			_focusedGraphic = focusedGraphic;
		}

		public Graphic FocusedGraphic
		{
			get { return _focusedGraphic; }
		}
	}
}
