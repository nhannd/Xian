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
using System.Drawing;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal class SliceLocationAnnotationItem : AnnotationItem
	{
		public SliceLocationAnnotationItem()
			: base("Dicom.ImagePlane.SliceLocation", new AnnotationResourceResolver(typeof(SliceLocationAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage is IImageSopProvider)
			{
				Frame frame = ((IImageSopProvider) presentationImage).Frame;
				Vector3D normal = frame.ImagePlaneHelper.GetNormalVector();
				Vector3D positionCenterOfImage = frame.ImagePlaneHelper.ConvertToPatient(new PointF((frame.Columns - 1) / 2F, (frame.Rows - 1) / 2F));

				if (normal != null && positionCenterOfImage != null)
				{
					// Try to be a bit more specific when we have spatial information
					// by showing directional information (L, R, H, F, A, P) as well as
					// the slice location.
					float absX = Math.Abs(normal.X);
					float absY = Math.Abs(normal.Y);
					float absZ = Math.Abs(normal.Z);

					// Get the primary direction based on the largest component of the normal.
					if (absZ >= absY && absZ >= absX)
					{
						//mostly axial because Z >= X and Y
						string directionString = (positionCenterOfImage.Z >= 0F) ? SR.ValueDirectionalMarkersHead : SR.ValueDirectionalMarkersFoot;
						return string.Format("{0}{1:F1}", directionString, Math.Abs(positionCenterOfImage.Z));
					}
					else if (absY >= absX && absY >= absZ)
					{
						//mostly coronal because Y >= X and Z
						string directionString = (positionCenterOfImage.Y >= 0F) ? SR.ValueDirectionalMarkersPosterior : SR.ValueDirectionalMarkersAnterior;
						return string.Format("{0}{1:F1}", directionString, Math.Abs(positionCenterOfImage.Y));
					}
					else
					{
						//mostly sagittal because X >= Y and Z
						string directionString = (positionCenterOfImage.X >= 0F) ? SR.ValueDirectionalMarkersLeft : SR.ValueDirectionalMarkersRight;
						return string.Format("{0}{1:F1}", directionString, Math.Abs(positionCenterOfImage.X));
					}
				}
			}

			return "";
		}
	}
}
