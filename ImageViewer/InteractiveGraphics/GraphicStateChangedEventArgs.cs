using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class GraphicStateChangedEventArgs : EventArgs
	{
		private IStatefulGraphic _statefulGraphic;
		private GraphicState _oldState;
		private GraphicState _newState;
		private IMouseInformation _mouseInformation;

		public GraphicStateChangedEventArgs()
		{

		}

		public IStatefulGraphic StatefulGraphic
		{
			get { return _statefulGraphic; }
			set { _statefulGraphic = value; }
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

		public IMouseInformation MouseInformation
		{
			get { return _mouseInformation; }
			set { _mouseInformation = value; }
		}
	}
}
