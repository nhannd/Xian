using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Presentation Lut Manager, which is responsible for managing installation and restoration
	/// of <see cref="IPresentationLut"/>s via the Memento pattern.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Implementors can maintain the named <see cref="IPresentationLut"/>s any way they choose.
	/// However, the <see cref="PresentationLutFactoryExtensionPoint"/> is the preferred method of 
	/// creating new <see cref="IPresentationLut"/>s.
	/// </para>
	/// <para>
	/// Implementors must not return null from the <see cref="GetLut"/> method.
	/// </para>
	/// </remarks>
	public interface IPresentationLutManager : IMemorable
	{
		/// <summary>
		/// Gets the currently installed <see cref="IPresentationLut"/>.
		/// </summary>
		/// <returns></returns>
		IPresentationLut GetLut();
		
		/// <summary>
		/// Installs an <see cref="IPresentationLut"/> by name.
		/// </summary>
		/// <param name="name">the name of the <see cref="IPresentationLut"/></param> to install.
		void InstallLut(string name);

		/// <summary>
		/// Installs an <see cref="IPresentationLut"/> by name, given the input <see cref="PresentationLutDescriptor"/>.
		/// </summary>
		/// <param name="descriptor">a <see cref="PresentationLutDescriptor"/> describing the <see cref="IPresentationLut"/> to be installed.</param>
		void InstallLut(PresentationLutDescriptor descriptor);

		/// <summary>
		/// Returns all available Presentation Luts in the form of <see cref="PresentationLutDescriptor"/>s.
		/// </summary>
		IEnumerable<PresentationLutDescriptor> AvailablePresentationLuts { get; }
	}
}
