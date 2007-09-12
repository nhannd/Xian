
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// The most basic of Linear Luts where the <see cref="WindowWidth"/> and <see cref="WindowCenter"/>
	/// can be set/manipulated.
	/// </summary>
	public interface IBasicVoiLutLinear : IVoiLutLinear
	{
		/// <summary>
		/// Gets or sets the Window Width.
		/// </summary>
		new double WindowWidth { get; set; }

		/// <summary>
		/// Gets or sets the Window Center.
		/// </summary>
		new double WindowCenter { get; set; }
	}
}
