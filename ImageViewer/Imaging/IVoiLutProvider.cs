
namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVoiLutProvider : IDrawable
	{
		IVoiLutManager VoiLutManager { get; }
	}
}
