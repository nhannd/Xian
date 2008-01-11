using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A specialization of the <see cref="BasicImageOperation"/> where the
	/// originator is an <see cref="IVoiLutManager"/>.
	/// </summary>
	public class VoiLutImageOperation : BasicImageOperation
	{
		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public VoiLutImageOperation(ApplyDelegate applyDelegate)
			: base(GetVoiLutManager, applyDelegate)
		{
		}

		/// <summary>
		/// Returns the <see cref="IVoiLutManager"/> associated with the 
		/// <see cref="IPresentationImage"/>, or null.
		/// </summary>
		/// <remarks>
		/// When used in conjunction with an <see cref="ImageOperationApplicator"/>,
		/// it is always safe to cast the return value directly to <see cref="IVoiLutManager"/>
		/// without checking for null from within the <see cref="BasicImageOperation.ApplyDelegate"/> 
		/// specified in the constructor.
		/// </remarks>
		public override IMemorable GetOriginator(IPresentationImage image)
		{
			return base.GetOriginator(image) as IVoiLutManager;
		}

		private static IMemorable GetVoiLutManager(IPresentationImage image)
		{
			if (image is IVoiLutProvider)
				return ((IVoiLutProvider)image).VoiLutManager;

			return null;
		}
	}
}
