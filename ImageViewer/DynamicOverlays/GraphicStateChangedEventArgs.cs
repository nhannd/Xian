using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class GraphicStateChangedEventArgs : EventArgs
	{
		private GraphicState _oldState;
		private GraphicState _newState;

		public GraphicStateChangedEventArgs()
		{

		}

		public GraphicState OldState
		{
			get { return _oldState; }
			set { _oldState = value; }
		}

		public GraphicState NewState
		{
			get { return _newState; }
			set 
			{
				Platform.CheckForNullReference(value, "NewState");
				_newState = value; 
			}
		}
	}
}
