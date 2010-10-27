#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	/// Used by the framework to relay information about the mouse to domain objects.
	/// </summary>
	public interface IMouseInformation
	{
		/// <summary>
		/// Gets the <see cref="ITile"/> the mouse is currently in.
		/// </summary>
		ITile Tile { get; }

		/// <summary>
		/// Gets the mouse's current location, in terms of the <see cref="ITile"/>'s client rectangle coordinates.
		/// </summary>
		Point Location { get; }

		/// <summary>
		/// Gets the currently depressed mouse button, if any.
		/// </summary>
		XMouseButtons ActiveButton { get; }

		/// <summary>
		/// Gets the current mouse button click count.
		/// </summary>
		uint ClickCount { get; }
	}
}
