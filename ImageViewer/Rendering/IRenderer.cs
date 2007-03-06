using System;
using System.Collections.Generic;
using System.Text;

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
		/// Draws the <see cref="IPresentationImage"/> passed in through the 
		/// <see cref="DrawArgs"/>.
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>
		/// This method is called by the <see cref="PresentationImage"/> whenever
		/// <see cref="IPresentationImage.Draw"/> is called.  If you are implementing
		/// your own renderer, <see cref="DrawArgs"/> contains all you need to 
		/// know to perform the rendering, such as the <see cref="IRenderingSurface"/>, the
		/// <see cref="IPresentationImage"/>, etc.  
		/// </remarks>
		void Draw(DrawArgs args);
	}
}
