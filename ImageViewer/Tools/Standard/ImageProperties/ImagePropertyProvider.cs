using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	public class ImagePropertyProviderExtensionPoint : ExtensionPoint<IImagePropertyProvider>
	{ }

	//TODO (cr Oct 2009): Properties window could be used for graphics, too
	public interface IImagePropertyProvider
	{
		IImageProperty[] GetProperties(IPresentationImage image);
	}
}