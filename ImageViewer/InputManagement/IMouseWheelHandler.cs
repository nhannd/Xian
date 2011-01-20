#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// An interface for objects that handle mouse wheel input.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The framework will look for this interface on <see cref="ClearCanvas.Desktop.Tools.ITool"/>s belonging to the current
	/// <see cref="IImageViewer"/> (via <see cref="IViewerShortcutManager.GetMouseWheelHandler"/>) and if an appropriate one 
	/// is found it will be given capture until a short period of time has expired.
	/// </para>
	/// </remarks>
	/// <seealso cref="IImageViewer"/>
	/// <seealso cref="ImageViewerComponent"/>
	/// <seealso cref="ITile"/>
	/// <seealso cref="TileController"/>
	public interface IMouseWheelHandler
	{
		/// <summary>
		/// Called by the framework when mouse wheel input has started.
		/// </summary>
		void StartWheel();

		/// <summary>
		/// Called by the framework each time the mouse wheel is moved.
		/// </summary>
		void Wheel(int wheelDelta);

		/// <summary>
		/// Called by the framework to indicate that mouse wheel activity has stopped 
		/// (a short period of time has elapsed without any activity).
		/// </summary>
		void StopWheel();
	}
}
