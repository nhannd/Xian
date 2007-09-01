using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public sealed class InitialVoiLutProviderExtensionPoint : ExtensionPoint<IInitialVoiLutProvider>
	{
		public InitialVoiLutProviderExtensionPoint()
		{
		}
	}

	public interface IInitialVoiLutProvider
	{
		ILut GetLut(IPresentationImage presentationImage);
	}
}
