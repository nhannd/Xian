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

using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	partial class DefineSlicePlaneTool
	{
		private static void SetSlicePlane(IPresentationImage image, IMprStandardSliceSet sliceSet, Vector3D startPoint, Vector3D endPoint)
		{
			IImageSopProvider imageSopProvider = image as IImageSopProvider;
			if (imageSopProvider == null)
				return;

			ImageOrientationPatient orientation = imageSopProvider.Frame.ImageOrientationPatient;
			Vector3D orientationRow = new Vector3D((float) orientation.RowX, (float) orientation.RowY, (float) orientation.RowZ);
			Vector3D orientationColumn = new Vector3D((float) orientation.ColumnX, (float) orientation.ColumnY, (float) orientation.ColumnZ);

			if (sliceSet != null && !sliceSet.IsReadOnly)
			{
				IImageBox imageBox = FindImageBox(sliceSet, image.ImageViewer as MprViewerComponent);
				IDisplaySet displaySet = imageBox.DisplaySet;
				imageBox.DisplaySet = null;

				sliceSet.SlicerParams = VolumeSlicerParams.CreateSlicing(sliceSet.Volume, orientationColumn, orientationRow, startPoint, endPoint);

				imageBox.DisplaySet = displaySet;

				IPresentationImage closestImage = GetClosestSlice(startPoint + (endPoint - startPoint)*2, displaySet);
				if (closestImage == null)
					imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count/2;
				else
					imageBox.TopLeftPresentationImage = closestImage;
				imageBox.DisplaySet.Draw();
			}
		}

		private static IPresentationImage GetClosestSlice(Vector3D positionPatient, IDisplaySet displaySet)
		{
			float closestDistance = float.MaxValue;
			IPresentationImage closestImage = null;

			foreach (IPresentationImage image in displaySet.PresentationImages)
			{
				if (image is IImageSopProvider)
				{
					Frame frame = (image as IImageSopProvider).Frame;
					Vector3D positionCenterOfImage = frame.ImagePlaneHelper.ConvertToPatient(new PointF((frame.Columns - 1)/2F, (frame.Rows - 1)/2F));
					Vector3D distanceVector = positionCenterOfImage - positionPatient;
					float distance = distanceVector.Magnitude;

					if (distance <= closestDistance)
					{
						closestDistance = distance;
						closestImage = image;
					}
				}
			}

			return closestImage;
		}
	}
}