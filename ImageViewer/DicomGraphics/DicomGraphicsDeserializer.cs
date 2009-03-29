using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DicomGraphics
{
	internal class DicomGraphicsDeserializationException : Exception
	{
		public DicomGraphicsDeserializationException(string message)
			: base(message)
		{
		}
	}

	internal class DicomGraphicsDeserializer
	{
		private readonly IDicomPresentationImage _image;

		public DicomGraphicsDeserializer(IDicomPresentationImage image)
		{
			_image = image;
		}

		public void Deserialize()
		{
			bool anyFailures = false;

			OverlayPlanesGraphic overlayPlanesGraphic = new OverlayPlanesGraphic();
			_image.DicomGraphics.Add(overlayPlanesGraphic);

			ShuttersGraphic shuttersGraphic = new ShuttersGraphic();
			_image.DicomGraphics.Add(shuttersGraphic);

			try
			{
				GeometricShuttersGraphic geometricShuttersGraphic = DicomGraphicsFactory.CreateGeometricShuttersGraphic(_image.Frame);
				shuttersGraphic.Graphics.Add(geometricShuttersGraphic);
			}
			catch (Exception e)
			{
				anyFailures = true;
				Platform.Log(LogLevel.Warn, e, "An error occurred trying to create geometric shutter graphics from the image header.");
			}

			try
			{
				List<OverlayPlaneGraphic> overlayPlaneGraphics = DicomGraphicsFactory.CreateOverlayPlaneGraphics(_image.Frame.ParentImageSop);
				foreach (OverlayPlaneGraphic graphic in overlayPlaneGraphics)
				{
					//Insert the bitmap shutter into the shutters graphic rather than the overlay planes.
					if (graphic.IsBitmapShutter)
						shuttersGraphic.Graphics.Insert(0, graphic);
					else
						overlayPlanesGraphic.Graphics.Add(graphic);
				}
			}
			catch (Exception e)
			{
				anyFailures = true;
				Platform.Log(LogLevel.Warn, e, "An error occurred trying to create overlay graphics from the image header.");
			}

			if (anyFailures)
				throw new DicomGraphicsDeserializationException(
					"At least one failure occurred in deserializing graphics from the image header.");
		}
	}
}
