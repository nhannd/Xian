#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
