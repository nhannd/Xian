using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.DynamicOverlays
{
	public class GraphicStateChangedEventArgs : EventArgs
	{
		private GraphicState m_OldState;
		private GraphicState m_NewState;

		public GraphicStateChangedEventArgs()
		{

		}

		public GraphicState OldState
		{
			get { return m_OldState; }
			set { m_OldState = value; }
		}

		public GraphicState NewState
		{
			get { return m_NewState; }
			set 
			{
				Platform.CheckForNullReference(value, "NewState");
				m_NewState = value; 
			}
		}
	}
}
