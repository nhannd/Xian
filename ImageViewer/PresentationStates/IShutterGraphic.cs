using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// Defines properties to control the presentation display of a graphic which may be used as a display shutter.
	/// </summary>
	public interface IShutterGraphic : IGraphic
	{
		/// <summary>
		/// Gets or sets the 16-bit grayscale presentation value which should replace the shuttered pixels.
		/// </summary>
		/// <remarks>
		/// The display of shuttered pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="PresentationValue"/> and <see cref="PresentationColor"/>
		/// properties allows this display to be customized by client code.
		/// </remarks>
		ushort PresentationValue { get; set; }

		/// <summary>
		/// Gets or sets the presentation color which should replace the shuttered pixels.
		/// </summary>
		/// <remarks>
		/// The display of shuttered pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="PresentationValue"/> and <see cref="PresentationColor"/>
		/// properties allows this display to be customized by client code.
		/// </remarks>
		Color PresentationColor { get; set; }
	}
}