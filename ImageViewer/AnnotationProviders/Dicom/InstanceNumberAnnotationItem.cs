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

            Frame frame = provider.Frame;

            string str;
			
			if (frame.ParentImageSop.ParentSeries != null)
			{
				//TODO: figure out how to do this without the parent series!
				str = String.Format("{0}/{1}", frame.ParentImageSop.InstanceNumber, frame.ParentImageSop.ParentSeries.Sops.Count);
			}
			else
			{
				str = frame.ParentImageSop.InstanceNumber.ToString();
			}

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
