using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	/// <summary>
	/// Describes whatever Lut is currently applied to a presentation image.
	/// </summary>
	/// <remarks>
	/// At first glance, you might think this belongs in the Dicom namespace within this project.
	/// However, the information in the Dicom header only applies to the initial presentation of
	/// the image and is not representative of the current state of the image in the viewport.
	/// The user could have changed the W/L, applied a custom Data Lut, or a Lut from a related
	/// Grayscale Presentation State object could be applied.
	/// </remarks>
	internal sealed class AppliedLutAnnotationItem : ResourceResolvingAnnotationItem
	{
		public AppliedLutAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Presentation.AppliedLut", ownerProvider)
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return string.Empty;

			IVOILUTLinearProvider image = presentationImage as IVOILUTLinearProvider;

			if (image == null)
				return string.Empty;

			if (image.VoiLutLinear == null)
				return string.Empty;

			return String.Format("{0}/{1}", image.VoiLutLinear.WindowWidth, image.VoiLutLinear.WindowCenter);
		}
	}
}
