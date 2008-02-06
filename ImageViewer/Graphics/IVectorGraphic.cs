using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a vector based graphic.
	/// </summary>
	public interface IVectorGraphic : IGraphic
	{
		/// <summary>
		/// Gets or sets the colour.
		/// </summary>
		Color Color { get; set; }

		/// <summary>
		/// Gets or sets the line style.
		/// </summary>
		LineStyle LineStyle { get; set; }
	}
}