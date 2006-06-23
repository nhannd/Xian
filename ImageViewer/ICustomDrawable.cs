using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for IDrawable.
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
