using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class MouseButtonMessage : IInputMessage
	{
		public enum ButtonActions { Down, Up };

		private ButtonActions _buttonAction;
		private Point _location; 
		private MouseButtonShortcut _mouseButtonShortcut;

		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction)
			: this(location, mouseButton, buttonAction, false, false, false)
		{
		}

		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction, bool control, bool alt, bool shift)
		{
			_location = location;
			_buttonAction = buttonAction;
			_mouseButtonShortcut = new MouseButtonShortcut(mouseButton, control, alt, shift);
		}

		private MouseButtonMessage()
		{
		}

		public Point Location
		{
			get { return _location; }
		}

		public ButtonActions ButtonAction
		{
			get { return _buttonAction; }
		}

		public MouseButtonShortcut Shortcut
		{
			get { return _mouseButtonShortcut; }
		}
	}
}
