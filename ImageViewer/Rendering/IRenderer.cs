#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Defines an <see cref="IPresentationImage"/> renderer.
	/// </summary>
	/// <remarks>
	/// Unless you are implementing your own renderer, you should never
	/// have to interact with this interface.  The two methods on <see cref="IRenderer"/>
	/// should only ever have to be called by the Framework, and thus
	/// should be treated as internal.
	/// </remarks>
	public interface IRenderer : IDisposable
	{
		/// <summary>
		/// Gets an <see cref="IRenderingSurface"/>.
		/// </summary>
		/// <param name="windowID">The window ID.  On Windows systems, this is the window handle
		/// or "hwnd".</param>
		/// <param name="width">The width of the surface.</param>
		/// <param name="height">The height of the surface.</param>
		/// <returns></returns>
		/// <remarks>
		/// This method is called by <b>TileControl</b> (i.e., the <see cref="ITile"/> view)
		/// whenever it is resized, which includes when the control is first created.
		/// Once <b>TileControl</b> has obtained the surface, it just holds onto it.
		/// Your implementation of <see cref="Draw"/> should just render to the
		/// that same surface (passed in via the <see cref="DrawArgs"/>) irrespective 
		/// of the <see cref="IPresentationImage"/> being rendered.
		/// </remarks>
		IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height);

		/// <summary>
		/// Renders the specified scene graph to the graphics surface.
		/// </summary>
		/// <remarks>
		/// Calling code should take care to handle any exceptions in a manner suitable to the context of
		/// the rendering operation. For example, the view control for an
		/// <see cref="ITile"/> may wish to display the error message in the tile's client area <i>without
		/// crashing the control</i>, whereas an image export routine may wish to notify the user via an error
		/// dialog and have the export output <i>fail to be created</i>. Automated routines (such as unit
		/// tests) may even wish that the exception bubble all the way to the top for debugging purposes.
		/// </remarks>
		/// <param name="args">A <see cref="DrawArgs"/> object that specifies the graphics surface and the scene graph to be rendered.</param>
		/// <exception cref="RenderingException">Thrown if any <see cref="Exception"/> is encountered in the rendering pipeline.</exception>
		void Draw(DrawArgs args);
	}
}
