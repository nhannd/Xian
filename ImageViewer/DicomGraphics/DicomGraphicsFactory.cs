using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.DicomGraphics
{
	internal class DicomGraphicsFactory
	{
		public static List<OverlayPlaneGraphic> CreateOverlayPlaneGraphics(ImageSop imageSop)
		{
			return CreateOverlayPlaneGraphics(new OverlayPlaneModuleIod(imageSop.DataSource));
		}

		public static List<OverlayPlaneGraphic> CreateOverlayPlaneGraphics(OverlayPlaneModuleIod overlaysIod)
		{
			List<OverlayPlaneGraphic> overlayPlaneGraphics = new List<OverlayPlaneGraphic>();

			BitmapDisplayShutterModuleIod bitmapShutterIod = new BitmapDisplayShutterModuleIod(overlaysIod.DicomAttributeProvider);
			bool bitmapShutterValid = (bitmapShutterIod.ShutterShape == ShutterShape.Bitmap);

			int bitmapShutterIndex = -1;
			if (bitmapShutterValid)
				bitmapShutterIndex = (int)(bitmapShutterIod.ShutterOverlayGroup - 0x6000)/2;

			foreach (OverlayPlane overlay in overlaysIod)
			{
				if (overlay.OverlayType != OverlayType.None)
				{
					bool isBitmapShutter = overlay.Index == bitmapShutterIndex;
					OverlayPlaneGraphic overlayPlaneGraphic = new OverlayPlaneGraphic(overlay, isBitmapShutter);
					if (isBitmapShutter)
					{
						if (bitmapShutterIod.ShutterPresentationValue != null)
							overlayPlaneGraphic.GrayPresentationValue = (ushort)bitmapShutterIod.ShutterPresentationValue;

						overlayPlaneGraphic.Color = null;
					}

					overlayPlaneGraphics.Add(overlayPlaneGraphic);
				}
			}

			return overlayPlaneGraphics;
		}

		public static GeometricShuttersGraphic CreateGeometricShuttersGraphic(Frame frame)
		{
			//TODO: you can actually have shutters defined per-frame, but right now we don't support per-frame data.

			DisplayShutterMacroIod shutterIod = new DisplayShutterMacroIod(frame.ParentImageSop.DataSource);
			return CreateGeometricShuttersGraphic(shutterIod, frame.Rows, frame.Columns);
		}

		public static GeometricShuttersGraphic CreateGeometricShuttersGraphic(DisplayShutterMacroIod shutterModule, int imageRows, int imageColumns)
		{
			GeometricShuttersGraphic shuttersGraphic = new GeometricShuttersGraphic(imageRows, imageColumns);
			if ((shutterModule.ShutterShape & ShutterShape.Circular) == ShutterShape.Circular)
			{
				Point? center = shutterModule.CenterOfCircularShutter;
				int? radius = shutterModule.RadiusOfCircularShutter;
				if (center != null && radius != null)
					shuttersGraphic.AddDicomShutter(new CircularShutter(center.Value, radius.Value));
			}

			if ((shutterModule.ShutterShape & ShutterShape.Rectangular) == ShutterShape.Rectangular)
			{
				int? left = shutterModule.ShutterLeftVerticalEdge;
				int? right = shutterModule.ShutterRightVerticalEdge;
				int? top = shutterModule.ShutterUpperHorizontalEdge;
				int? bottom = shutterModule.ShutterLowerHorizontalEdge;

				if (left != null && right != null && top != null && bottom != null)
					shuttersGraphic.AddDicomShutter(new RectangularShutter(left.Value, right.Value, top.Value, bottom.Value));
			}

			if ((shutterModule.ShutterShape & ShutterShape.Polygonal) == ShutterShape.Polygonal)
			{
				Point[] points = shutterModule.VerticesOfThePolygonalShutter;
				shuttersGraphic.AddDicomShutter(new PolygonalShutter(points));
			}

			return shuttersGraphic;
		}
	}
}
