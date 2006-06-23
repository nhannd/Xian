using System;
using System.Drawing;
using ClearCanvas.Common;



namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for IRenderer.
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
