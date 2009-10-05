namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVoiLutInstaller
	{
		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>The Voi Lut as an <see cref="IComposableLut"/>.</returns>
		IComposableLut VoiLut { get; }

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="voiLut">The Lut to be installed.</param>
		void InstallVoiLut(IComposableLut voiLut);

		/// <summary>
		/// Gets or sets whether the output of the VOI LUT should be inverted for display.
		/// </summary>
		bool Invert { get; set; }
	}
}
