using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public sealed class PresentationLutOperationApplicator : ImageOperationApplicator
	{
		/// <summary>
		/// Initializes a new instance of <see cref="PresentationLutOperationApplicator"/>
		/// with the specified <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="image">an image</param>
		public PresentationLutOperationApplicator(IPresentationImage image)
			: base(image)
		{ 
		}

		/// <summary>
		/// Gets the <see cref="IPresentationLutManager"/> which should be the originator for <b>all</b> Presentation Lut undoable operations.
		/// </summary>
		/// <param name="image">The image the memento is associated with</param>
		/// <returns>the <see cref="IPresentationLutManager"/> which should be the originator for <b>all</b> Presentation Lut undoable operation.</returns>
		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			IPresentationLutProvider provider = image as IPresentationLutProvider;
			if (provider == null)
				throw new Exception("Presentation image does not support IPresentationLutProvider");

			return provider.PresentationLutManager;
		}
	}
}
