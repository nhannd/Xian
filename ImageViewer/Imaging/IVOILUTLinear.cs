
namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVoiLutLinear : ILut
	{
		double WindowWidth { get; }
		double WindowCenter { get; }
	}
}
