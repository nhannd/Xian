using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public class MouseWheelHandler : IMouseWheelHandler
	{
		private delegate void MouseWheelDelegate();

		private MouseWheelDelegate _wheelIncrementDelegate;
		private MouseWheelDelegate _wheelDecrementDelegate;

		public MouseWheelHandler(object target, string wheelIncrementDelegateName, string wheelDecrementDelegateName)
		{
			_wheelIncrementDelegate = (MouseWheelDelegate)Delegate.CreateDelegate(typeof(MouseWheelDelegate), target, wheelIncrementDelegateName);
			_wheelDecrementDelegate = (MouseWheelDelegate)Delegate.CreateDelegate(typeof(MouseWheelDelegate), target, wheelDecrementDelegateName);
		}

		private MouseWheelHandler()
		{ 
		}

		#region IMouseWheelHandler Members

		public void Activate(int wheelDelta)
		{
			if (wheelDelta < 0)
				_wheelIncrementDelegate();
			else
				_wheelDecrementDelegate();
		}

		#endregion
	}
}
