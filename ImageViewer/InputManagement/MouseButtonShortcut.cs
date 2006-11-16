using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class MouseButtonShortcut
	{
		private XMouseButtons _mouseButton;
		private Modifiers _modifiers;

		public MouseButtonShortcut(XMouseButtons mouseButton, ModifierFlags modifiers)
		{
			_mouseButton = mouseButton;
			_modifiers = new Modifiers(modifiers);
		}

		public MouseButtonShortcut(XMouseButtons mouseButton, bool control, bool alt, bool shift)
		{
			_mouseButton = mouseButton;
			_modifiers = new Modifiers(control, alt, shift);
		}

		public MouseButtonShortcut(XMouseButtons mouseButton)
			: this(mouseButton, false, false, false)
		{
		}
		
		private MouseButtonShortcut()
		{ 
		}

		public XMouseButtons MouseButton
		{
			get { return _mouseButton; }
		}

		public Modifiers Modifiers
		{
			get { return _modifiers; }
		}

		public override bool Equals(object obj)
		{
			if (obj is MouseButtonShortcut)
			{
				MouseButtonShortcut shortcut = (MouseButtonShortcut)obj;
				return shortcut.MouseButton == this.MouseButton && shortcut.Modifiers.Equals(this.Modifiers);
			}

			return false;
		}

		public override int GetHashCode()
		{
			int returnvalue = 7;
			returnvalue = 11 * returnvalue + _modifiers.GetHashCode();
			returnvalue = 11 * returnvalue + _mouseButton.GetHashCode();
			return returnvalue;
		}
	}
}
