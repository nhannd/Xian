using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Do not implement.  Will be removed in 1.0.
	/// </summary>
	public interface IDrawable
	{
		/// <summary>
		/// Draws the object.
		/// </summary>
		/// <param name="paintNow"><b>true</b> to draw immediately, 
		/// <b>false</b> to invalidate the object and delay painting until the next paint cycle.</param>
		void Draw(bool paintNow);
	}
}
