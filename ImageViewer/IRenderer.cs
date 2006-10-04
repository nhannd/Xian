using System;
using System.Drawing;
using ClearCanvas.Common;



namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Do not implement.  Will be removed in 1.0.
	/// </summary>
	public interface IRenderer : IDisposable
	{
		void Draw(Graphics graphics, ImageDrawingEventArgs e);
		void Paint(Graphics graphics, Rectangle repaintArea);
	}

    [ExtensionPoint()]
    public class RendererExtensionPoint : ExtensionPoint<IRenderer>
    {
    }


}
