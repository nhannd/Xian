#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// A message object created by the view layer to allow a controlling object 
	/// (e.g. <see cref="TileController"/>) to handle mouse button messages.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="MouseButtonShortcut"/>
	/// <seealso cref="TileController"/>
	public sealed class MouseButtonMessage
	{
		/// <summary>
		/// An enum used to indicate the button state.
		/// </summary>
		public enum ButtonActions
		{
			/// <summary>
			/// Indicates that the button is down.
			/// </summary>
			Down,

			/// <summary>
			/// Indicates that the button has been released.
			/// </summary>
			Up
		};


		private readonly ButtonActions _buttonAction;
		private readonly Point _location; 
		private readonly MouseButtonShortcut _mouseButtonShortcut;
		private readonly uint _clickCount;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction, uint clickCount, bool control, bool alt, bool shift)
		{
			_location = location;
			_buttonAction = buttonAction;
			_clickCount = clickCount;
			_mouseButtonShortcut = new MouseButtonShortcut(mouseButton, control, alt, shift);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction, uint clickCount, ModifierFlags modifierFlags)
			: this(location, mouseButton, buttonAction, clickCount, 
						(modifierFlags & ModifierFlags.Control) == ModifierFlags.Control,
						(modifierFlags & ModifierFlags.Alt) == ModifierFlags.Alt,
						(modifierFlags & ModifierFlags.Shift) == ModifierFlags.Shift)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction, uint clickCount)
			: this(location, mouseButton, buttonAction, clickCount, false, false, false)
		{
		}

		/// <summary>
		/// Gets the mouse location.
		/// </summary>
		public Point Location
		{
			get { return _location; }
		}

		/// <summary>
		/// Gets the current button state.
		/// </summary>
		public ButtonActions ButtonAction
		{
			get { return _buttonAction; }
		}

		/// <summary>
		/// Gets the associated <see cref="MouseButtonShortcut"/>.
		/// </summary>
		public MouseButtonShortcut Shortcut
		{
			get { return _mouseButtonShortcut; }
		}

		/// <summary>
		/// Gets the current mouse button click count.
		/// </summary>
		public uint ClickCount
		{
			get { return _clickCount; }
		}
	}
}
