using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IPresentationLutManager : IMemorable
	{
		IPresentationLut GetLut();
		
		void InstallLut(string name);
		
		void InstallLut(PresentationLutDescriptor descriptor);

		IEnumerable<PresentationLutDescriptor> AvailablePresentationLuts { get; }
	}
}
