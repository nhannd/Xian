
namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IPresentationLutProvider : IDrawable
	{
		IPresentationLutManager PresentationLutManager { get; }
	}
}
