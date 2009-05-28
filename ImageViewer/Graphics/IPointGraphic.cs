using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that can be defined as a single point.
	/// </summary>
	public interface IPointGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets or sets the point associated with this graphic.
		/// </summary>
		PointF Point { get; set; }

		/// <summary>
		/// Occurs when the value of <see cref="Point"/> changes.
		/// </summary>
		event EventHandler PointChanged;
	}
}