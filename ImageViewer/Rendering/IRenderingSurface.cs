using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Rendering
{
	public interface IRenderingSurface : IDisposable
	{
		IntPtr WindowID
		{
			get;
		}

		IntPtr ContextID
		{
			get;
			set;
		}
	}
}
