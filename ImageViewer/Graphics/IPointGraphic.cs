using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface IPointGraphic : IVectorGraphic
	{
		PointF Point { get; set; }
		event EventHandler PointChanged;
	}
}