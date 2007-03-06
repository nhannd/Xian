using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Creates and restores window/level mementos across all linked images.
	/// </summary>
	public class WindowLevelApplicator : ImageOperationApplicator
	{
		/// <summary>
		/// Initializes a new instance of <see cref="WindowLevelApplicator"/>
		/// with the specified <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="image"></param>
		public WindowLevelApplicator(IPresentationImage image)
			: base(image)
		{

		}

		/// <summary>
		/// Gets the <see cref="IVOILUTLinear"/> from which the memento came.
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			IVOILUTLinearProvider voiLutLinear = image as IVOILUTLinearProvider;

			if (voiLutLinear == null)
				throw new Exception("PresentationImage does not support IVOILUTLinear");

			return voiLutLinear.VoiLutLinear as IMemorable;
		}
	}
}
