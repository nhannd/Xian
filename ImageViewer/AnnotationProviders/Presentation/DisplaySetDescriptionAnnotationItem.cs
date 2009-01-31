using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal class DisplaySetDescriptionAnnotationItem : AnnotationItem
	{
		public DisplaySetDescriptionAnnotationItem() 
			: base("Presentation.DisplaySetDescription", new AnnotationResourceResolver(typeof(DFOVAnnotationItem).Assembly))
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
}
