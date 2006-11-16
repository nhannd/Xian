using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class MouseButtonControlAttribute : Attribute
	{
		private MouseButtonShortcut _shortcut;

		public MouseButtonControlAttribute(XMouseButtons mouseButton, ModifierFlags modifierFlags)
		{
			_shortcut = new MouseButtonShortcut(mouseButton, modifierFlags);
		}

		private MouseButtonControlAttribute()
		{
		}

		public MouseButtonShortcut ShortCut
		{
			get { return _shortcut; }
		}
	}
}