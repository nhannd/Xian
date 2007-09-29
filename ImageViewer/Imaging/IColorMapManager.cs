using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Color Map Manager, which is responsible for managing installation and restoration
	/// of <see cref="IColorMap"/>s via the Memento pattern.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Implementors can maintain the named <see cref="IColorMap"/>s any way they choose.
	/// However, the <see cref="ColorMapFactoryExtensionPoint"/> is the preferred method of 
	/// creating new <see cref="IColorMap"/>s.
	/// </para>
	/// <para>
	/// Implementors must not return null from the <see cref="GetColorMap"/> method.
	/// </para>
	/// </remarks>
	public interface IColorMapManager : IMemorable
	{
		/// <summary>
		/// Gets the currently installed <see cref="IColorMap"/>.
		/// </summary>
		/// <returns></returns>
		IColorMap GetColorMap();

		/// <summary>
		/// Installs an <see cref="IColorMap"/> by name.
		/// </summary>
		/// <param name="name">the name of the <see cref="IColorMap"/></param> to install.
		void InstallColorMap(string name);

		/// <summary>
		/// Installs an <see cref="IColorMap"/> by name, given the input <see cref="ColorMapDescriptor"/>.
		/// </summary>
		/// <param name="descriptor">a <see cref="ColorMapDescriptor"/> describing the <see cref="IColorMap"/> to be installed.</param>
		void InstallColorMap(ColorMapDescriptor descriptor);

		/// <summary>
		/// Returns all available Color Maps in the form of <see cref="ColorMapDescriptor"/>s.
		/// </summary>
		IEnumerable<ColorMapDescriptor> AvailableColorMaps { get; }
	}
}
