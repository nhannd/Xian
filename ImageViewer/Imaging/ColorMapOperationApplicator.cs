using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// An <see cref="ImageOperationApplicator"/> for <see cref="IColorMap"/>s.  The Originator 
	/// for this class (returned by <see cref="GetOriginator"/> is the <see cref="IPresentationImage"/>'s 
	/// <see cref="IColorMapManager"/>, if applicable.
	/// </summary>
	public sealed class ColorMapOperationApplicator : ImageOperationApplicator
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ColorMapOperationApplicator"/>
		/// with the specified <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="image">an image</param>
		public ColorMapOperationApplicator(IPresentationImage image)
			: base(image)
		{ 
		}

		/// <summary>
		/// Gets the <see cref="IColorMapManager"/> which should be the originator for <b>all</b> Color Map undoable operations.
		/// </summary>
		/// <remarks>
		/// The <see cref="IPresentationImage"/> must implement <see cref="IColorMapProvider"/> or an exception is thrown.
		/// </remarks>
		/// <param name="image">The image the memento is associated with</param>
		/// <returns>the <see cref="IColorMapManager"/> which should be the originator for <b>all</b> Color Map undoable operation.</returns>
		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			IColorMapProvider provider = image as IColorMapProvider;
			if (provider == null)
				throw new Exception("Presentation image does not support IColorMapProvider");

			return provider.ColorMapManager;
		}
	}
}
