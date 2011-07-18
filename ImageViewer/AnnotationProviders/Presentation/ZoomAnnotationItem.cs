#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal sealed class ZoomAnnotationItem : AnnotationItem
	{
		public ZoomAnnotationItem()
			: base("Presentation.Zoom", new AnnotationResourceResolver(typeof(ZoomAnnotationItem).Assembly))
		{ 
		
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return string.Empty;

			ISpatialTransformProvider image = presentationImage as ISpatialTransformProvider;
			if (image  == null)
				return string.Empty;

			return String.Format(SR.FormatZoomFactor, image.SpatialTransform.Scale.ToString("F2"));
		}
	}
}
