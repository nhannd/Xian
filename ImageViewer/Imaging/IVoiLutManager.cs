using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Voi Lut Manager, which is responsible for managing installation and restoration
	/// of Voi Luts via the Memento pattern.
	/// </summary>
	/// <remarks>
	/// Implementors must not return null from the <see cref="GetLut"/> method.
	/// </remarks>
	public interface IVoiLutManager : IMemorable
	{
		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>the Voi Lut as an <see cref="IComposableLut"/></returns>
		IComposableLut GetLut();

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="lut">the lut to be installed</param>
		void InstallLut(IComposableLut lut);
	}
}
