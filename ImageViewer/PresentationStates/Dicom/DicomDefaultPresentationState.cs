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
		internal DicomDefaultPresentationState() : base() {}

		private static void Deserialize(IDicomPresentationImage _image)
		{
			bool anyFailures = false;

			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(_image, true);
			if(dicomGraphicsPlane == null)
				throw new DicomGraphicsDeserializationException("Unknown exception.");

			// Check if the image header specifies a bitmap display shutter
			BitmapDisplayShutterModuleIod bitmapShutterIod = new BitmapDisplayShutterModuleIod(_image.ImageSop.DataSource);
			int bitmapShutterIndex = -1;
			if (bitmapShutterIod.ShutterShape == ShutterShape.Bitmap)
				bitmapShutterIndex = bitmapShutterIod.ShutterOverlayGroupIndex;
			if (bitmapShutterIndex < 0 || bitmapShutterIndex > 15)
				bitmapShutterIndex = -1;

			try
			{
				GeometricShuttersGraphic geometricShuttersGraphic = DicomGraphicsFactory.CreateGeometricShuttersGraphic(_image.Frame);
				dicomGraphicsPlane.Shutters.Add(geometricShuttersGraphic);
			}
			catch (Exception e)
			{
				anyFailures = true;
				Platform.Log(LogLevel.Warn, e, "An error occurred trying to create geometric shutter graphics from the image header.");
			}

			try
			{
				List<OverlayPlaneGraphic> overlayPlaneGraphics = DicomGraphicsFactory.CreateOverlayPlaneGraphics(_image.Frame);
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
			throw new NotSupportedException();
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

		public override void Clear(IEnumerable<IPresentationImage> image) {}
	}
}