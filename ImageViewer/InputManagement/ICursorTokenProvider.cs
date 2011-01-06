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
	/// A provider of a <see cref="CursorToken"/> that is returned based on the current mouse position within an <see cref="ITile"/>.
	/// </summary>
	/// <remarks>
	/// The framework will look for this interface on graphic objects (<see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>) 
	/// in the current <see cref="IPresentationImage"/>'s SceneGraph (see <see cref="PresentationImage.SceneGraph"/>) when the
	/// mouse has moved within the current <see cref="ITile"/>.  If the object returns a <see cref="CursorToken"/>, then the
	/// corresponding cursor will be shown at the current mouse position.
	/// </remarks>
	/// <seealso cref="CursorToken"/>
	/// <seealso cref="ITile"/>
	/// <seealso cref="IPresentationImage"/>
	/// <seealso cref="PresentationImage.SceneGraph"/>
	/// <seealso cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>
	/// <seealso cref="ClearCanvas.ImageViewer.Graphics.Graphic"/>
	public interface ICursorTokenProvider
	{
		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		CursorToken GetCursorToken(Point point);
	}
}
