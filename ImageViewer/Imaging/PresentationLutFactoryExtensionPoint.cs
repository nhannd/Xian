using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IPresentationLutFactory
	{
		string Name { get; }
		string Description { get; }

		IPresentationLut Create();
	}

	public class PresentationLutFactoryExtensionPoint : ExtensionPoint<IPresentationLutFactory>
	{
	}
}
