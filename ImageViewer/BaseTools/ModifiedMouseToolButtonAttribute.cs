using System;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.BaseTools
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ModifiedMouseToolButtonAttribute : Attribute
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