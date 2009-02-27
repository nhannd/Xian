using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Common interface for regions of interest that have the notion of perimeter
	/// </summary>
	public interface IRoiPerimeterProvider
	{
		/// <summary>
		/// Gets the perimeter of the <see cref="Roi"/> in pixels.
		/// </summary>
		double PixelPerimeter { get; }

		/// <summary>
		/// Gets the perimeter of the <see cref="Roi"/> in millimetres.
		/// </summary>
		/// <exception cref="UncalibratedImageException">If the image has no pixel spacing
		/// information and has not been calibrated.</exception>
		double Perimeter { get; }

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information or has
		/// previously been calibrated, and hence the <see cref="Perimeter"/> property is available.
		/// </summary>
		bool IsCalibrated { get; }
	}
}