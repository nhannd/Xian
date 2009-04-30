#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
