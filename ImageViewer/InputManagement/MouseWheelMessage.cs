using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class MouseWheelMessage : IInputMessage
	{
		private int _wheelDelta;
		private MouseWheelShortcut _wheelShortcut;

		public MouseWheelMessage(int wheelDelta, bool control, bool alt, bool shift)
		{
			_wheelDelta = wheelDelta;
			_wheelShortcut = new MouseWheelShortcut(control, alt, shift);
		}

		public MouseWheelMessage(int wheelDelta)
			: this(wheelDelta, false, false, false)
		{
		}

		private MouseWheelMessage()
		{
		}

		public int WheelDelta
		{
			get { return _wheelDelta; }
		}

		public MouseWheelShortcut Shortcut
		{
			get { return _wheelShortcut; }
		}
	}
}
