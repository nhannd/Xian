using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Presentation Lut as defined for use by the framework.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Presentation Luts are often the same for many images, so they are stored internally
	/// by the framework and used by the <see cref="LutComposer"/> when creating the <see cref="OutputLut"/>
	/// for a particular image.
	/// </para>
	/// <para>
	/// Presentation Luts must implement <see cref="IEquatable{T}"/> so that a new Presentation Lut can 
	/// be compared with those already stored and discarded if an equivalent one already exists.
	/// </para>
	/// </remarks>
	public interface IPresentationLut : ILut, IEquatable<IPresentationLut>
	{
		/// <summary>
		/// Gets or sets whether or not the Presentation Lut should be inverted.
		/// </summary>
		bool Invert { get; set; }
	}
}
