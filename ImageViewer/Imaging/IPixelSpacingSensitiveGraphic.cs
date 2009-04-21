using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Interface identifying that a particular <see cref="IGraphic"/> is dependent on the
	/// <see cref="Frame.PixelSpacing"/> of the parent image.
	/// </summary>
	public interface IPixelSpacingSensitiveGraphic : IGraphic
	{
		/// <summary>
		/// Forces a recomputation of the values of the graphic that are dependent on the
		/// <see cref="Frame.PixelSpacing"/> of the parent image.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Calibration tools that change the <see cref="Frame.PixelSpacing"/> for a given
		/// image should check for top-level graphics that implement this interface and invoke
		/// this method to force a recomputation.
		/// </para>
		/// <para>
		/// Graphics that implement this interface should assume that the source image's pixel
		/// spacing has changed since the last computation, and that any derived values should
		/// be recomputed at this time.
		/// </para>
		/// </remarks>
		void Refresh();
	}
}