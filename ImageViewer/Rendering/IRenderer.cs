using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Rendering
{
	public interface IRenderer : IDisposable
	{
		IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height);
		void Draw(DrawArgs args);
	}
}
