using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Creates and restores Luts across all images, including automatically determined Luts (e.g. from the Dicom Header).
	/// </summary>
	public class AutoVoiLutOperationApplicator : ImageOperationApplicator
	{
		/// <summary>
		/// Initializes a new instance of <see cref="AutoVoiLutOperationApplicator"/>
		/// with the specified <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="image">an image</param>
		public AutoVoiLutOperationApplicator(IPresentationImage image)
			: base(image)
		{ 
		}

		/// <summary>
		/// Gets the <see cref="IAutoLutApplicator"/> from which the memento originally came.
		/// </summary>
		/// <param name="image">The image the memento is associated with</param>
		/// <returns>the <see cref="IAutoLutApplicator"/> from which the memento originally came</returns>
		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			IAutoVoiLutApplicatorProvider provider = image as IAutoVoiLutApplicatorProvider;
			if (provider == null)
				throw new Exception("Presentation image does not support IAutoLutApplicator");

			return provider.AutoVoiLutApplicator as IMemorable;
		}
	}
}
