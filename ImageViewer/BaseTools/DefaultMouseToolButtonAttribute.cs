using System;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// An attribute used by <see cref="MouseImageViewerTool"/> to specify it's default <see cref="MouseButtonShortcut"/>.
	/// </summary>
	/// <seealso cref="MouseButtonShortcut"/>
	/// <seealso cref="MouseImageViewerTool"/>
	/// <seealso cref="IViewerShortcutManager"/>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DefaultMouseToolButtonAttribute : Attribute
	{
		private readonly MouseButtonShortcut _shortcut;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DefaultMouseToolButtonAttribute(XMouseButtons mouseButton)
		{
			_shortcut = new MouseButtonShortcut(mouseButton);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public DefaultMouseToolButtonAttribute(XMouseButtons mouseButton, ModifierFlags modifierFlags)
		{
			_shortcut = new MouseButtonShortcut(mouseButton, modifierFlags);
		}

		/// <summary>
		/// Gets the associated <see cref="MouseButtonShortcut"/>.
		/// </summary>
		public MouseButtonShortcut Shortcut
		{
			get { return _shortcut; }
		}
	}
}
