using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Common interface for regions of interest that have the notion of length.
	/// </summary>
	public interface IRoiLengthProvider
	{
		/// <summary>
		/// Gets the length of the <see cref="Roi"/> in pixels.
		/// </summary>
		double PixelLength { get; }

		/// <summary>
		/// Gets the length of the <see cref="Roi"/> in millimetres.
		/// </summary>
		/// <exception cref="UncalibratedImageException">If the image has no pixel spacing
		/// information and has not been calibrated.</exception>
		double Length { get; }

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information or has
		/// previously been calibrated, and hence the <see cref="Length"/> property is available.
		/// </summary>
		bool IsCalibrated { get; }
	}
}