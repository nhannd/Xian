using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Common interface for regions of interest that have the notion of area.
	/// </summary>
	public interface IRoiAreaProvider
	{
		/// <summary>
		/// Gets the area of the <see cref="Roi"/> in square pixels.
		/// </summary>
		double PixelArea { get; }

		/// <summary>
		/// Gets the area of the <see cref="Roi"/> in square millimetres.
		/// </summary>
		/// <exception cref="UncalibratedImageException">If the image has no pixel spacing
		/// information and has not been calibrated.</exception>
		double Area { get; }

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information or has
		/// previously been calibrated, and hence the <see cref="Area"/> property is available.
		/// </summary>
		bool IsCalibrated { get; }
	}
}