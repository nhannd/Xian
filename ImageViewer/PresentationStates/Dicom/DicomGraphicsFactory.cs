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
					if (overlay.OverlayType != OverlayType.None && !overlay.IsEmbedded)
					{
						try
						{
							foreach (int overlayFrame in overlay.GetRelevantOverlayFrames(frame.FrameNumber, frame.ParentImageSop.NumberOfFrames))
							{
								OverlayData od = new OverlayData(overlay.TryComputeOverlayDataBitOffset(overlayFrame),
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
					if (dictionary.ReferencesFrame(frame.ParentImageSop.SopInstanceUID, frame.FrameNumber))
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