
namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IBasicVoiLutLinear : IVoiLutLinear
	{
		double WindowWidth { get; set; }
		double WindowCenter { get; set; }
	}
}
