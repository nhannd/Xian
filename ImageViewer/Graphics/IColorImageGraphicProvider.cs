namespace ClearCanvas.ImageViewer.Graphics
{
	public interface IColorImageGraphicProvider
	{
		/// <summary>
		/// Gets an <see cref="ColorImageGraphic"/>.
		/// </summary>
		ColorImageGraphic ImageGraphic { get; }
	}
}
