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

		private DicomDefaultPresentationState() {}

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
					if (bitmapShutterIndex != -1 && overlay.Index == bitmapShutterIndex)
					{
						// Someday when we support CIELab colour, we should set presentation value/colour based on client display type
						if (bitmapShutterIod.ShutterPresentationValue != null)
							overlay.GrayPresentationValue = (ushort)bitmapShutterIod.ShutterPresentationValue;
						overlay.Color = null;

						// insert the bitmap shutter into the shutters graphic instead of with the other overlays
						dicomGraphicsPlane.Shutters.Add(overlay);
					}
					else if (overlay.Index >= 0 && overlay.Index < 16)
					{
						// otherwise just add the overlay to the default layer for overlays and activate immediately
						dicomGraphicsPlane.ImageOverlays.Add(overlay);
						dicomGraphicsPlane.ImageOverlays.ActivateAsLayer(overlay, "OVERLAY");
					}
					else
					{
						// otherwise just add the overlay to the default layer for overlays and activate immediately
						dicomGraphicsPlane.UserOverlays.Add(overlay);
						dicomGraphicsPlane.UserOverlays.ActivateAsLayer(overlay, "OVERLAY");
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
