using System;
using System.Drawing;

namespace ClearCanvas.Workstation.Model
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
