namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides focus support.
	/// </summary>
	public interface IFocussableGraphic : IGraphic
	{
		/// <summary>
		/// Gets or set a value indicating whether the <see cref="IGraphic"/> is in focus.
		/// </summary>
		bool Focussed { get; set; }
	}
}
