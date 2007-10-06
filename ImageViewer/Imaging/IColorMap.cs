using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Color Map as defined for use by the framework.  This corresponds to the Dicom 
	/// concept of a <b>Palette Color Lut Transformation</b> for grayscale images, but
	/// has been named <b>IColorMap</b> for simplicity.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Color Maps are often the same for many images, so they are stored internally
	/// by the framework when rendering the image.
	/// </para>
	/// <para>
	/// Color Maps must implement <see cref="IEquatable{T}"/> so that a new Color Map can 
	/// be compared with those already stored and discarded if an equivalent one already exists.
	/// </para>
	/// </remarks>
	public interface IColorMap : IComposableLut, IEquatable<IColorMap>
	{
		/// <summary>
		/// Gets the map's data.  This property should be considered readonly
		/// and is only provided for fast (unsafe) iteration over the array.
		/// </summary>
		int[] Data { get; }
	}
}
