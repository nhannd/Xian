using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Creates and restores spatial transform mementos across all linked images.
	/// </summary>
	public class SpatialTransformApplicator : ImageOperationApplicator
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SpatialTransformApplicator"/>
		/// with the specified <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="image"></param>
		public SpatialTransformApplicator(IPresentationImage image)
			: base(image)
		{

		}

		/// <summary>
		/// Gets the <see cref="ISpatialTransform"/> from which the memento came.
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			ISpatialTransformProvider spatialTransformProvider = image as ISpatialTransformProvider;

			if (spatialTransformProvider == null)
				throw new Exception("PresentationImage does not support ISpatialTransformProvider");

			return spatialTransformProvider.SpatialTransform as IMemorable;
		}
	}
}
