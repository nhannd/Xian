using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal class InstanceNumberAnnotationItem : AnnotationItem
	{
		public InstanceNumberAnnotationItem()
			: base("Dicom.GeneralImage.InstanceNumber", new AnnotationResourceResolver(typeof(InstanceNumberAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			IImageSopProvider provider = presentationImage as IImageSopProvider;
			if (provider == null)
				return "";

			int totalImages = 1;
			if (presentationImage.ParentDisplaySet != null)
				totalImages = presentationImage.ParentDisplaySet.PresentationImages.Count;

			Frame frame = provider.Frame;
			string str = String.Format("{0}/{1}",
								frame.ParentImageSop.InstanceNumber,
								totalImages);

			if (frame.ParentImageSop.NumberOfFrames > 1)
			{
				string frameString = String.Format(
					"Fr: {0}/{1}",
					frame.FrameNumber,
					frame.ParentImageSop.NumberOfFrames);

				str += " " + frameString;
			}

			return str;
		}
	}
}
