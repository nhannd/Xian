using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface ITextGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		string Text { get; set; }

		/// <summary>
		/// Gets or sets the size in points.
		/// </summary>
		/// <remarks>
		/// Default value is 10 points.
		/// </remarks>
		float SizeInPoints { get; set; }

		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <remarks>
		/// Default value is "Arial".
		/// </remarks>
		string Font { get; set; }

		/// <summary>
		/// Gets the dimensions of text's bounding box.
		/// </summary>
		/// <remarks>
		SizeF Dimensions { get; }

		PointF Location { get; set; }
	}
}