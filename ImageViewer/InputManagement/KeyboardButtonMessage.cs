#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// A message object created by the view layer to allow a controlling object 
	/// (e.g. <see cref="TileController"/>) to handle keyboard button messages.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="KeyboardButtonShortcut"/>
	/// <seealso cref="TileController"/>
	public sealed class KeyboardButtonMessage
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
		private readonly KeyboardButtonShortcut _buttonShortcut;

		/// <summary>
		/// Constructor.
		/// </summary>
		public KeyboardButtonMessage(XKeys keyData, ButtonActions buttonAction)
		{
			_buttonAction = buttonAction;
			_buttonShortcut = new KeyboardButtonShortcut(keyData);
		}

		/// <summary>
		/// Gets the button state.
		/// </summary>
		public ButtonActions ButtonAction
		{
			get { return _buttonAction; }
		}

		/// <summary>
		/// Gets the associated <see cref="KeyboardButtonShortcut"/>.
		/// </summary>
		public KeyboardButtonShortcut Shortcut
		{
			get { return _buttonShortcut; }
		}
	}
}
