#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[ExtensionOf(typeof (AnnotationItemProviderExtensionPoint))]
	public class FusionImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public FusionImageAnnotationItemProvider()
			: base("AnnotationItemProviders.AdvancedImaging.Fusion", SR.LabelAdvancedImageFusion)
		{
			_annotationItems = new List<IAnnotationItem>();
			_annotationItems.Add(new MismatchedFrameOfReferenceFusionImageAnnotationItem());
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}

		internal class MismatchedFrameOfReferenceFusionImageAnnotationItem : AnnotationItem
		{
			public MismatchedFrameOfReferenceFusionImageAnnotationItem()
				: base("AdvancedImaging.Fusion.MismatchedFrameOfReference", @"LabelMismatchedFrameOfReference", @"LabelMismatchedFrameOfReference") {}

			public override string GetAnnotationText(IPresentationImage presentationImage)
			{
				if (presentationImage is FusionPresentationImage)
				{
					var fusionImage = (FusionPresentationImage) presentationImage;
					if (fusionImage.OverlayFrameData.BaseFrame.FrameOfReferenceUid != fusionImage.OverlayFrameData.OverlayFrameOfReferenceUid)
					{
						return SR.CodeMismatchedFrameOfReference;
					}
				}
				return string.Empty;
			}
		}
	}
}