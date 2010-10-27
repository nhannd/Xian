#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Factory class to create the individual graphics components of a DICOM image for presentation.
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	internal class DicomGraphicsFactory
	{
		#region DICOM Overlays (and Bitmap Shutters)

		public static List<OverlayPlaneGraphic> CreateOverlayPlaneGraphics(Frame frame)
		{
			return CreateOverlayPlaneGraphics(frame, null);
		}

		public static List<OverlayPlaneGraphic> CreateOverlayPlaneGraphics(Frame frame, OverlayPlaneModuleIod overlaysFromPresentationState)
		{
			ISopDataSource dataSource = frame.ParentImageSop.DataSource;
			OverlayPlaneModuleIod overlaysIod = new OverlayPlaneModuleIod(dataSource);

			List<OverlayPlaneGraphic> overlayPlaneGraphics = new List<OverlayPlaneGraphic>();

			bool failedOverlays = false;

			foreach (OverlayPlane overlay in overlaysIod)
			{
				if (overlay.OverlayType != OverlayType.None)
				{
					try
					{
						foreach (int overlayFrame in overlay.GetRelevantOverlayFrames(frame.FrameNumber, frame.ParentImageSop.NumberOfFrames))
						{
							byte[] overlayData = dataSource.GetFrameData(frame.FrameNumber).GetNormalizedOverlayData(overlay.Index + 1, overlayFrame + 1);

							OverlayPlaneGraphic overlayPlaneGraphic = new OverlayPlaneGraphic(overlay, overlayData, OverlayPlaneSource.Image);
							overlayPlaneGraphics.Add(overlayPlaneGraphic);
						}
					}
					catch (Exception ex)
					{
						failedOverlays = true;
						Platform.Log(LogLevel.Warn, ex, "Failed to load overlay in image header.");
					}
				}
			}

			if (overlaysFromPresentationState != null)
			{
				foreach (OverlayPlane overlay in overlaysFromPresentationState)
				{
					// it is highly unlikely that an overlay should be embedded in the pixel data of an image-less instance, but worth the sanity check anyway
					if (overlay.OverlayType != OverlayType.None && !overlay.IsEmbedded)
					{
						try
						{
							foreach (int overlayFrame in overlay.GetRelevantOverlayFrames(frame.FrameNumber, frame.ParentImageSop.NumberOfFrames))
							{
								int bitOffset;
								overlay.TryComputeOverlayDataBitOffset(overlayFrame, out bitOffset);

								OverlayData od = new OverlayData(bitOffset,
								                                 overlay.OverlayRows, overlay.OverlayColumns,
								                                 overlay.IsBigEndianOW, overlay.OverlayData);
								OverlayPlaneGraphic overlayPlaneGraphic = new OverlayPlaneGraphic(overlay, od.Unpack(), OverlayPlaneSource.PresentationState);
								overlayPlaneGraphics.Add(overlayPlaneGraphic);
							}
						}
						catch (Exception ex)
						{
							failedOverlays = true;
							Platform.Log(LogLevel.Warn, ex, "Failed to load overlay from softcopy presentation state.");
						}
					}
				}
			}

			if (failedOverlays)
			{
				Platform.Log(LogLevel.Warn, "At least one overlay failed to load.");
			}

			return overlayPlaneGraphics;
		}

		#endregion

		#region DICOM Shutters (Geometric)

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

		#endregion

		#region DICOM Graphic Annotations

		public static IEnumerable<DicomGraphicAnnotation> CreateGraphicAnnotations(Frame frame, GraphicAnnotationModuleIod annotationsFromPresentationState, RectangleF displayedArea)
		{
			List<DicomGraphicAnnotation> list = new List<DicomGraphicAnnotation>();

			GraphicAnnotationSequenceItem[] annotationSequences = annotationsFromPresentationState.GraphicAnnotationSequence;
			if (annotationSequences != null)
			{
				foreach (GraphicAnnotationSequenceItem sequenceItem in annotationSequences)
				{
					ImageSopInstanceReferenceDictionary dictionary = new ImageSopInstanceReferenceDictionary(sequenceItem.ReferencedImageSequence, true);
					if (dictionary.ReferencesFrame(frame.ParentImageSop.SopInstanceUid, frame.FrameNumber))
					{
						list.Add(new DicomGraphicAnnotation(sequenceItem, displayedArea));
					}
				}
			}

			return list.AsReadOnly();
		}

		#endregion
	}
}