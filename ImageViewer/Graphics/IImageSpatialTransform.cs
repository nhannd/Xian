namespace ClearCanvas.ImageViewer.Graphics
{
	public interface IImageSpatialTransform : ISpatialTransform
	{
		bool ScaleToFit { get; set; }
	}
}
