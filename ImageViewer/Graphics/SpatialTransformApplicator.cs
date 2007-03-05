using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer;

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
			ISpatialTransformProvider voiLutLinear = image as ISpatialTransformProvider;

			if (voiLutLinear == null)
				throw new Exception("PresentationImage does not support ISpatialTransformProvider");

			return voiLutLinear.SpatialTransform as IMemorable;
		}
	}
}
