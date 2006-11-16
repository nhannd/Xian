using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public delegate void MouseWheelDelegate();

	public class MouseWheelDelegatePair
	{
		private MouseWheelDelegate _wheelIncrementDelegate;
		private MouseWheelDelegate _wheelDecrementDelegate;

		public MouseWheelDelegatePair(MouseWheelDelegate wheelIncrementDelegate, MouseWheelDelegate wheelDecrementDelegate)
		{
			_wheelIncrementDelegate = wheelIncrementDelegate;
			_wheelDecrementDelegate = wheelDecrementDelegate;
		}

		private MouseWheelDelegatePair()
		{ 
		}

		public MouseWheelDelegate WheelIncrementDelegate
		{
			get { return _wheelIncrementDelegate; }
		}

		public MouseWheelDelegate WheelDecrementDelegate
		{
			get { return _wheelDecrementDelegate; }
		}
	}
}
