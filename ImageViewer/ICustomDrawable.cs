using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Do not implement.  Will be removed in 1.0.
	/// </summary>
	public interface ICustomDrawable
	{
		bool DoubleBuffer
		{
			get;
			set;
		}

		void Draw(IntPtr contextID, Rectangle clientArea);
	}
}
