using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for events where a rectangle has changed.
	/// </summary>
	public class RectangleChangedEventArgs : EventArgs
	{
		private RectangleF _rectangle;

		/// <summary>
		/// Instantiates a new instance of <see cref="RectangleChangedEventArgs"/>
		/// with a specified rectangle.
		/// </summary>
		/// <param name="rectangle"></param>
		public RectangleChangedEventArgs(RectangleF rectangle)
		{
			_rectangle = rectangle;
		}

		/// <summary>
		/// Gets the rectangle.
		/// </summary>
		public RectangleF Rectangle
		{
			get { return _rectangle; }
		}
	}
}
