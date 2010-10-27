#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// A message object created by the view layer to allow a controlling object 
	/// (e.g. <see cref="TileController"/>) to handle mouse wheel messages.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="MouseWheelShortcut"/>
	/// <seealso cref="TileController"/>
	public sealed class MouseWheelMessage
	{
		private readonly int _wheelDelta;
		private readonly MouseWheelShortcut _wheelShortcut;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelMessage(int wheelDelta, bool control, bool alt, bool shift)
		{
			_wheelDelta = wheelDelta;
			_wheelShortcut = new MouseWheelShortcut(control, alt, shift);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelMessage(int wheelDelta)
			: this(wheelDelta, false, false, false)
		{
		}

		/// <summary>
		/// Gets the wheel delta.
		/// </summary>
		public int WheelDelta
		{
			get { return _wheelDelta; }
		}

		/// <summary>
		/// Gets the associated <see cref="MouseWheelShortcut"/>.
		/// </summary>
		public MouseWheelShortcut Shortcut
		{
			get { return _wheelShortcut; }
		}
	}
}
