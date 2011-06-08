#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public class ThumbnailDescriptor
    {
        public ThumbnailDescriptor(IDisplaySet displaySet, IPresentationImage referenceImage)
        {
            DisplaySet = displaySet;
            ReferenceImage = referenceImage;
        }

        public IDisplaySet DisplaySet { get; private set; }
        public IPresentationImage ReferenceImage { get; private set; }

        public string Identifier
        { 
            get
            {
                var referenceImageUid = GetReferenceImageUid();
                if (String.IsNullOrEmpty(referenceImageUid))
                    return DisplaySet.Uid;

                return String.Format("{0}/{1}", DisplaySet.Uid, referenceImageUid);
            }
        }

        private string GetReferenceImageUid()
        {
            if (ReferenceImage == null)
                return String.Empty;

 	        if (!String.IsNullOrEmpty(ReferenceImage.Uid))
 	            return ReferenceImage.Uid;

            if (!(ReferenceImage is IImageSopProvider))
                return String.Empty;

            var frame = ((IImageSopProvider) ReferenceImage).Frame;
            return String.Format("{0}:{1}", frame.SopInstanceUid, frame.FrameNumber);
        }

        internal static IPresentationImage GetMiddlePresentationImage(IDisplaySet displaySet)
        {
            if (displaySet.PresentationImages.Count == 0)
                return null;

            if (displaySet.PresentationImages.Count <= 2)
                return displaySet.PresentationImages[0];

            return displaySet.PresentationImages[displaySet.PresentationImages.Count / 2];
        }
    }
}