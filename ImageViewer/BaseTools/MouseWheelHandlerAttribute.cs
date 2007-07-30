using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.BaseTools
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class MouseWheelHandlerAttribute : Attribute
	{
		private MouseWheelShortcut _shortcut;
		private uint _stopDelayMilliseconds;

		public MouseWheelHandlerAttribute()
			: this(ModifierFlags.None)
		{
		}

		public MouseWheelHandlerAttribute(ModifierFlags modifiers)
		{
			_shortcut = new MouseWheelShortcut(modifiers);
			_stopDelayMilliseconds = 0;
		}

		public MouseWheelShortcut Shortcut
		{
			get { return _shortcut; }
		}

		public uint StopDelayMilliseconds
		{
			get { return _stopDelayMilliseconds; }
			set { _stopDelayMilliseconds = value; }
		}
	}
}
