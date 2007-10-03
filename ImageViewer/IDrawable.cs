using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides drawing functionality.
	/// </summary>
	public interface IDrawable
	{
		/// <summary>
		/// Fires just before the object is actually drawn/rendered.
		/// </summary>
		event EventHandler Drawing;

		/// <summary>
		/// Draw the object.
		/// </summary>
		void Draw();
	}
}
