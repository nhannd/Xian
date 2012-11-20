#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Policy;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
    public class KeyImageDisplaySet
    {
        public static void AddKeyImage(IPresentationImage image)
        {
            Platform.CheckForNullReference(image, "image");
            Platform.CheckForNullReference(image.ImageViewer, "image.ImageViewer");

            if (!PermissionsHelper.IsInRole(AuthorityTokens.KeyImages))
                throw new PolicyException(SR.ExceptionCreateKeyImagePermissionDenied);

            var sopProvider = image as IImageSopProvider;
            if (sopProvider == null)
                throw new ArgumentException("The image must be an IImageSopProvider.", "image");

            IDisplaySet displaySet = null;

            foreach (var set in image.ImageViewer.LogicalWorkspace.ImageSets)
            {
                foreach (var d in set.DisplaySets)
                {
                    var displaySetDescriptor = d.Descriptor as KeyImageDisplaySetDescriptor;

                    if (displaySetDescriptor != null 
                        && displaySetDescriptor.SourceStudy.StudyInstanceUid.Equals(sopProvider.Sop.StudyInstanceUid))
                    {
                        displaySet = d;
                        break;
                    }
                }
                if (displaySet != null)
                    break;
            }

            if (displaySet == null)
            {
                var displaySetDescriptor = new KeyImageDisplaySetDescriptor(new StudyIdentifier(sopProvider.ImageSop));
                displaySet = new DisplaySet(displaySetDescriptor);
                image.ImageViewer.LogicalWorkspace.ImageSets[0].DisplaySets.Add(displaySet);
            }

            var presentationImage = image.CreateFreshCopy();
            
            var presentationState = DicomSoftcopyPresentationState.Create(image);
            var basicImage = presentationImage as BasicPresentationImage;
            if (basicImage != null)
                basicImage.PresentationState = presentationState;

            displaySet.PresentationImages.Add(presentationImage);

            foreach (var imageBox in image.ImageViewer.PhysicalWorkspace.ImageBoxes)
            {
                if (imageBox.DisplaySet != null && imageBox.DisplaySet.Descriptor.Uid == displaySet.Descriptor.Uid)
                {
                    var physicalImage = presentationImage.CreateFreshCopy();

                    presentationState = DicomSoftcopyPresentationState.Create(image);
                    basicImage = physicalImage as BasicPresentationImage;
                    if (basicImage != null)
                        basicImage.PresentationState = presentationState;

                    imageBox.DisplaySet.PresentationImages.Add(physicalImage);

                    imageBox.Draw();
                }
            }
        }

        public static void RemoveKeyImage(IPresentationImage image)
        {
            Platform.CheckForNullReference(image, "image");
            Platform.CheckForNullReference(image.ImageViewer, "image.ImageViewer");

            if (!PermissionsHelper.IsInRole(AuthorityTokens.KeyImages))
                throw new PolicyException(SR.ExceptionCreateKeyImagePermissionDenied);
            foreach (var imageSet in image.ImageViewer.LogicalWorkspace.ImageSets)
            {
                foreach (var d in imageSet.DisplaySets)
                {
                    var displaySetDescriptor = d.Descriptor as KeyImageDisplaySetDescriptor;

                    if (displaySetDescriptor != null)
                    {
                        foreach (var i in d.PresentationImages)
                        {
                            if (i.Uid == image.Uid)
                            {
                                
                            }                            
                        }
                    }
                }
            }
        }
    }
}
