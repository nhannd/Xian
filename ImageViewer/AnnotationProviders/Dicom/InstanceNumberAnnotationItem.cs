#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
