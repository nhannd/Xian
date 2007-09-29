
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A read-only Linear Voi Lut, where the <see cref="WindowWidth"/> and <see cref="WindowCenter"/> cannot be set.
	/// </summary>
	public interface IVoiLutLinear : IComposableLut
	{
		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		double WindowWidth { get; }

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		double WindowCenter { get; }
	}
}
