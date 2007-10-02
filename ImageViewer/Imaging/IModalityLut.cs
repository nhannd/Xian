using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Modality Lut as defined for use by the framework.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Modality Luts are often the same for many images, so they are stored internally
	/// by the framework and used by the <see cref="LutComposer"/> when creating the <see cref="ComposedLut"/>
	/// for a particular image.
	/// </para>
	/// <para>
	/// Modality Luts must implement <see cref="IEquatable{T}"/> so that a new Modality Lut can 
	/// be compared with those already stored and discarded if an equivalent one already exists.
	/// </para>
	/// </remarks>
	public interface IModalityLut : IComposableLut, IEquatable<IModalityLut>
	{
	}
}
