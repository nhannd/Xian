using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	public class ImagePropertyProviderExtensionPoint : ExtensionPoint<IImagePropertyProvider>
	{ }

	public interface IImagePropertyProvider
	{
		IImageProperty[] GetProperties(IPresentationImage image);
	}
}