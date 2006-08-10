using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class GraphicStateChangedEventArgs : EventArgs
	{
		private GraphicState _OldState;
		private GraphicState _NewState;

		public GraphicStateChangedEventArgs()
		{

		}

		public GraphicState OldState
		{
			get { return _OldState; }
			set { _OldState = value; }
		}

		public GraphicState NewState
		{
			get { return _NewState; }
			set 
			{
				Platform.CheckForNullReference(value, "NewState");
				_NewState = value; 
			}
		}
	}
}
