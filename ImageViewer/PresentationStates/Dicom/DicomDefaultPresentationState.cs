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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// DICOM images without a real, concrete presentation state SOP should
	/// install this pseudo presentation state to recover shutter and overlay
	/// components from the image header.
	/// </summary>
	[Cloneable(true)]
	internal sealed class DicomDefaultPresentationState : PresentationState
	{
		internal static readonly DicomDefaultPresentationState Instance = new DicomDefaultPresentationState();

		private DicomDefaultPresentationState()
		{}

		private static void Deserialize(IDicomPresentationImage image)
		{
			bool anyFailures = false;

			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
			if(dicomGraphicsPlane == null)
				throw new DicomGraphicsDeserializationException("Unknown exception.");

			// Check if the image header specifies a bitmap display shutter
			BitmapDisplayShutterModuleIod bitmapShutterIod = new BitmapDisplayShutterModuleIod(image.ImageSop.DataSource);
			int bitmapShutterIndex = -1;
			if (bitmapShutterIod.ShutterShape == ShutterShape.Bitmap)
				bitmapShutterIndex = bitmapShutterIod.ShutterOverlayGroupIndex;
			if (bitmapShutterIndex < 0 || bitmapShutterIndex > 15)
				bitmapShutterIndex = -1;

			try
			{
				GeometricShuttersGraphic geometricShuttersGraphic = DicomGraphicsFactory.CreateGeometricShuttersGraphic(image.Frame);
				dicomGraphicsPlane.Shutters.Add(geometricShuttersGraphic);
			}
			catch (Exception e)
			{
				anyFailures = true;
				Platform.Log(LogLevel.Warn, e, "An error occurred trying to create geometric shutter graphics from the image header.");
			}

			try
			{
				List<OverlayPlaneGraphic> overlayPlaneGraphics = DicomGraphicsFactory.CreateOverlayPlaneGraphics(image.Frame);
				foreach (OverlayPlaneGraphic overlay in overlayPlaneGraphics)
				{
					if (overlay.Index == bitmapShutterIndex)
					{
						// Someday when we support CIELab colour, we should set presentation value/colour based on client display type
						if (bitmapShutterIod.ShutterPresentationValue != null)
							overlay.GrayPresentationValue = (ushort)bitmapShutterIod.ShutterPresentationValue;
						overlay.Color = null;

						// insert the bitmap shutter into the shutters graphic instead of with the other overlays
						dicomGraphicsPlane.Shutters.Add(overlay);
					}
					else
					{
						// otherwise just add the overlay to the default layer for overlays and activate immediately
						dicomGraphicsPlane.ImageOverlays.Add(overlay);
						dicomGraphicsPlane.ImageOverlays.ActivateAsLayer(overlay, "OVERLAY");
					}
				}
			}
			catch (Exception e)
			{
				anyFailures = true;
				Platform.Log(LogLevel.Warn, e, "An error occurred trying to create overlay graphics from the image header.");
			}

			dicomGraphicsPlane.Shutters.ActivateFirst();

			if (anyFailures)
				throw new DicomGraphicsDeserializationException("At least one failure occurred in deserializing graphics from the image header.");
		}

		public override void Serialize(IEnumerable<IPresentationImage> images)
		{
			throw new NotSupportedException("The default presentation state cannot be serialized.");
		}

		public override void Deserialize(IEnumerable<IPresentationImage> images)
		{
			foreach (IPresentationImage image in images)
			{
				if (image is IDicomPresentationImage)
				{
					Deserialize((IDicomPresentationImage) image);
				}
			}
		}

		public override void Clear(IEnumerable<IPresentationImage> image)
		{
		}
	}
}
