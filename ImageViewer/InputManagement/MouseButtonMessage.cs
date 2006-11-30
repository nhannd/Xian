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
		private uint _clickCount;

		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction, uint clickCount, bool control, bool alt, bool shift)
		{
			_location = location;
			_buttonAction = buttonAction;
			_clickCount = clickCount;
			_mouseButtonShortcut = new MouseButtonShortcut(mouseButton, control, alt, shift);
		}

		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction, uint clickCount, ModifierFlags modifierFlags)
			: this(location, mouseButton, buttonAction, clickCount, 
						(modifierFlags & ModifierFlags.Control) == ModifierFlags.Control,
						(modifierFlags & ModifierFlags.Alt) == ModifierFlags.Alt,
						(modifierFlags & ModifierFlags.Shift) == ModifierFlags.Shift)
		{
		}

		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction, uint clickCount)
			: this(location, mouseButton, buttonAction, clickCount, false, false, false)
		{
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

		public uint ClickCount
		{
			get { return _clickCount; }
		}
	}
}
