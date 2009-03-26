using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics {
	public interface IPointsGraphic : IVectorGraphic 
	{
		IList<PointF> Points { get; }

		/// <summary>
		/// Gets the index of the next closest point.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		int IndexOfNextPoint(PointF point);

		event EventHandler PointsChanged;
		event EventHandler<ListEventArgs<PointF>> PointChanged;
	}
}
