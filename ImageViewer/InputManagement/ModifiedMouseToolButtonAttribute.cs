using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ModifiedMouseToolButtonAttribute : Attribute
	{
		private MouseButtonShortcut _shortcut;

		public ModifiedMouseToolButtonAttribute(XMouseButtons mouseButton, ModifierFlags modifierFlags)
		{
			_shortcut = new MouseButtonShortcut(mouseButton, modifierFlags);
		}

		private ModifiedMouseToolButtonAttribute()
		{
		}

		public MouseButtonShortcut Shortcut
		{
			get { return _shortcut; }
		}
	}
}