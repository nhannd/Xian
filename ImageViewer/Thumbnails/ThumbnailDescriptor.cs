#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public class ThumbnailDescriptor : IEquatable<ThumbnailDescriptor>
    {
        public ThumbnailDescriptor(string identifier, IPresentationImage referenceImage)
        {
            Platform.CheckForEmptyString(identifier, "identifier");
            Platform.CheckForNullReference(referenceImage, "referenceImage");
            Identifier = identifier;
            ReferenceImage = referenceImage;
        }

        public string Identifier { get; private set; }
        public IPresentationImage ReferenceImage { get; private set; }

        public override string ToString()
        {
            return Identifier;
        }

        public override bool Equals(object obj)
        {
            if (obj is ThumbnailDescriptor)
                return Equals((ThumbnailDescriptor)obj);

            return false;
        }

        public override int GetHashCode()
        {
            return 0x178F ^ Identifier.GetHashCode();
        }

        #region IEquatable<ThumbnailDescriptor> Members

        public bool Equals(ThumbnailDescriptor other)
        {
            return other != null && other.Identifier == Identifier;
        }

        #endregion

        #region Static Helpers

        public static ThumbnailDescriptor Create(IDisplaySet displaySet)
        {
            return Create(displaySet, false);
        }

        public static ThumbnailDescriptor Create(IDisplaySet displaySet, bool copyImage)
        {
            return Create(displaySet, GetMiddlePresentationImage(displaySet), copyImage);
        }

        public static ThumbnailDescriptor Create(IDisplaySet displaySet, IPresentationImage image)
        {
            return Create(displaySet, image, false);
        }

        public static ThumbnailDescriptor Create(IDisplaySet displaySet, IPresentationImage image, bool copyImage)
        {
            if (displaySet == null || image == null)
                return null;

            string identifier = GetIdentifier(displaySet, image);
            if (String.IsNullOrEmpty(identifier))
                return null;

            return new ThumbnailDescriptor(identifier, copyImage ? image.CreateFreshCopy() : image);
        }

        private static string GetIdentifier(IDisplaySet displaySet, IPresentationImage image)
        {
            var referenceImageUid = GetReferenceImageUid(image);
            if (String.IsNullOrEmpty(referenceImageUid))
                return displaySet.Uid;

            return String.Format("{0}/{1}", displaySet.Uid, referenceImageUid);
        }

        private static string GetReferenceImageUid(IPresentationImage referenceImage)
        {
            if (referenceImage == null)
                return String.Empty;

            if (!String.IsNullOrEmpty(referenceImage.Uid))
                return referenceImage.Uid;

            if (!(referenceImage is IImageSopProvider))
                return String.Empty;

            var frame = ((IImageSopProvider)referenceImage).Frame;
            return String.Format("{0}:{1}", frame.SopInstanceUid, frame.FrameNumber);
        }

        public static IPresentationImage GetMiddlePresentationImage(IDisplaySet displaySet)
        {
            if (displaySet.PresentationImages.Count == 0)
                return null;

            return displaySet.PresentationImages.Count <= 2 
                ? displaySet.PresentationImages[0] 
                : displaySet.PresentationImages[displaySet.PresentationImages.Count / 2];
        }

        #endregion
    }
}