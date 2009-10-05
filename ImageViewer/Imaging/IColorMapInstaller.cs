using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IColorMapInstaller
	{
		IDataLut ColorMap { get; }

		void InstallColorMap(string name);

		void InstallColorMap(ColorMapDescriptor descriptor);

		void InstallColorMap(IDataLut colorMap);

		IEnumerable<ColorMapDescriptor> AvailableColorMaps { get; }
	}
}
