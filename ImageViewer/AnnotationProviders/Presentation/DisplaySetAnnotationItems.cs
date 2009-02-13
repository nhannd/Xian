using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal class DisplaySetDescriptionAnnotationItem : AnnotationItem
	{
		public DisplaySetDescriptionAnnotationItem()
			: base("Presentation.DisplaySetDescription", new AnnotationResourceResolver(typeof(DisplaySetDescriptionAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage != null && presentationImage.ParentDisplaySet != null)
				return presentationImage.ParentDisplaySet.Description ?? "";
			else
				return "";
		}
	}
	internal class DisplaySetNumberAnnotationItem : AnnotationItem
	{
		public DisplaySetNumberAnnotationItem()
			: base("Presentation.DisplaySetNumber", new AnnotationResourceResolver(typeof(DisplaySetNumberAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage != null && presentationImage.ParentDisplaySet != null)
			{
				if (presentationImage.ParentDisplaySet.ParentImageSet != null)
				{
					return String.Format("{0}/{1}", presentationImage.ParentDisplaySet.Number,
						presentationImage.ParentDisplaySet.ParentImageSet.DisplaySets.Count);
				}
				else
				{
					return presentationImage.ParentDisplaySet.Number.ToString();
				}
			}
			else
			{
				return "";
			}
		}
	}
}
