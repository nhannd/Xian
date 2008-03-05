#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[MenuAction("activate", "global-menus/MenuTools/MenuSynchronization/MenuReferenceLines", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarSynchronization/ToolbarReferenceLines", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActiveChanged")]
	[Tooltip("activate", "TooltipReferenceLines")]
	[IconSet("activate", IconScheme.Colour, "Icons.CurrentReferenceLineToolSmall.png", "Icons.CurrentReferenceLineToolMedium.png", "Icons.CurrentReferenceLineToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Synchronization.ReferenceLines.Current")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ReferenceLineTool : ImageViewerTool
	{
		private struct ReferenceLineInfo
		{
			public readonly PointF StartPoint;
			public readonly PointF EndPoint;
			public readonly string Label;

			public ReferenceLineInfo(PointF startPoint, PointF endPoint, string label)
			{
				this.StartPoint = startPoint;
				this.EndPoint = endPoint;
				this.Label = label;
			}
		}

		private ImageInfo _currentReferenceImageInfo;
		private IPresentationImage _currentReferenceImage;
		
		private bool _active;
		private event EventHandler _activeChanged;

		private SynchronizationToolCoordinator _coordinator;
		private SopInfoCache _cache;

		private static readonly float _oneDegreeInRadians = (float)(1 * Math.PI / 180);

		public ReferenceLineTool()
		{
			_active = false;
		}

		public bool Active
		{
			get { return _active; }
			set
			{
				if (_active == value)
					return;

				_active = value;
				EventsHelper.Fire(_activeChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler ActiveChanged
		{
			add { _activeChanged += value; }
			remove { _activeChanged -= value; }
		}

		public override void Initialize()
		{
			base.Initialize();

			base.ImageViewer.EventBroker.ImageDrawing += OnImageDrawing;

			_cache = SopInfoCache.Get();
			
			_coordinator = SynchronizationToolCoordinator.Get(base.ImageViewer);
			_coordinator.ReferenceLineTool = this;
		}

		protected override void Dispose(bool disposing)
		{
			base.ImageViewer.EventBroker.ImageDrawing -= OnImageDrawing;

			_cache.Release();
			_coordinator.Release();

			base.Dispose(disposing);
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			RefreshTargetImage(e.PresentationImage);
		}

		public void Toggle()
		{
			Active = !Active;
			_coordinator.OnReferenceLinesRefreshed(GetRefreshedImages());
		}

		private ReferenceLineCompositeGraphic GetReferenceLineCompositeGraphic(IPresentationImage image)
		{
			return _coordinator.GetReferenceLineCompositeGraphic(image);
		}

		private void SetCurrentReferenceImage()
		{
			if (_currentReferenceImage == this.SelectedPresentationImage)
				return;

			_currentReferenceImage = this.SelectedPresentationImage;

			bool valid = false;
			if (_currentReferenceImage != null && _currentReferenceImage is IImageSopProvider)
			{
				Frame currentReferenceFrame = ((IImageSopProvider) _currentReferenceImage).Frame;
				if (!String.IsNullOrEmpty(currentReferenceFrame.FrameOfReferenceUid) && 
					!String.IsNullOrEmpty(currentReferenceFrame.ParentImageSop.StudyInstanceUID))
				{
					_currentReferenceImageInfo = _cache.GetImageInformation(((IImageSopProvider)_currentReferenceImage).Frame);
					valid = _currentReferenceImageInfo != null;
				}
			}

			if (!valid)
			{
				_currentReferenceImage = null;
				_currentReferenceImageInfo = null;
			}
		}

		private static List<Vector3D> GetPlaneIntersectionPoints(ImageInfo referenceImageInfo, ImageInfo targetImageInfo)
		{
			List<Vector3D> intersectionPoints = new List<Vector3D>();
			Vector3D[,] lineSegments = new Vector3D[,]
				{
					{ referenceImageInfo.PositionPatientTopLeft, referenceImageInfo.PositionPatientTopRight },
					{ referenceImageInfo.PositionPatientTopLeft, referenceImageInfo.PositionPatientBottomLeft },
					{ referenceImageInfo.PositionPatientBottomLeft, referenceImageInfo.PositionPatientBottomRight },
					{ referenceImageInfo.PositionPatientTopRight, referenceImageInfo.PositionPatientBottomRight }
				};

			for (int i = 0; i < 4; ++i)
			{
				Vector3D intersection = Vector3D.GetIntersectionOfLineSegmentWithPlane(targetImageInfo.Normal,
																		targetImageInfo.PositionPatientCenterOfImage, 
																		lineSegments[i, 0], lineSegments[i, 1], true);

				if (intersection != null)
					intersectionPoints.Add(intersection);
			}

			return intersectionPoints;
		}

		private ReferenceLineInfo? GetReferenceLineInfo(Frame referenceFrame, Frame targetFrame)
		{
			ImageInfo referenceImageInfo = _cache.GetImageInformation(referenceFrame);
			ImageInfo targetImageInfo = _cache.GetImageInformation(targetFrame);

			List<Vector3D> intersectionPoints = GetPlaneIntersectionPoints(referenceImageInfo, targetImageInfo);
			// most of the time there will be exactly 2 (or zero) points of intersection, however it is possible for
			// the plane to intersect the exact corners of the reference image, in which case there would be 4 points of intersection.
			if (intersectionPoints.Count < 2)
				return null;

			Vector3D transformed1 = targetFrame.ImagePlaneHelper.ConvertToImage(intersectionPoints[0]);
			Vector3D transformed2 = targetFrame.ImagePlaneHelper.ConvertToImage(intersectionPoints[1]);

			//The coordinates need to be converted to pixel coordinates because right now they are in mm.
			PointF point1 = (PointF)targetFrame.ImagePlaneHelper.ConvertToImagePixel(new PointF(transformed1.X, transformed1.Y));
			PointF point2 = (PointF)targetFrame.ImagePlaneHelper.ConvertToImagePixel(new PointF(transformed2.X, transformed2.Y));

			return new ReferenceLineInfo(point1, point2, referenceFrame.ParentImageSop.InstanceNumber.ToString());
		}

		private void GetFirstAndLastReferenceLineInfo(IPresentationImage targetImage, 
						out ReferenceLineInfo? firstReferenceLineInfo,
						out ReferenceLineInfo? lastReferenceLineInfo)
		{
			Frame currentReferenceFrame = ((IImageSopProvider)_currentReferenceImage).Frame;
			Frame targetFrame = ((IImageSopProvider)targetImage).Frame;

			firstReferenceLineInfo = lastReferenceLineInfo = null;

			float firstReferenceImageZComponent = float.MaxValue;
			float lastReferenceImageZComponent = float.MinValue;

			// 1. Find all images in the same plane as the current reference image.
			foreach (IPresentationImage image in _currentReferenceImage.ParentDisplaySet.PresentationImages)
			{
				if (image is IImageSopProvider)
				{
					Frame frame = ((IImageSopProvider)image).Frame;
					if (frame.FrameOfReferenceUid == currentReferenceFrame.FrameOfReferenceUid &&
						frame.ParentImageSop.StudyInstanceUID == currentReferenceFrame.ParentImageSop.StudyInstanceUID)
					{
						ImageInfo info = _cache.GetImageInformation(frame);
						if (info != null)
						{
							// 2. Is the image within 1 degree of being in the same plane as the current image?
							float angle = Math.Abs((float)Math.Acos(info.Normal.Dot(_currentReferenceImageInfo.Normal)));
							if (angle <= _oneDegreeInRadians || (Math.PI - angle) <= _oneDegreeInRadians)
							{
								// 3. Use the Image Position (in the coordinate system of the Image Plane without moving the origin!) 
								//    to determine the first and last reference line.  By transforming the Image Position (Patient) to 
								//    the coordinate system of the image plane, we can then simply take the 2 images with
								//    the smallest and largest z-components, respectively, as the 'first' and 'last' reference images.
								float imageZComponent = info.PositionImagePlaneTopLeft.Z;

								// < keeps the first image as close to the beginning of the display set as possible.
								if (imageZComponent < firstReferenceImageZComponent)
								{
									ReferenceLineInfo? referenceLineInfo = GetReferenceLineInfo(frame, targetFrame);
									if (referenceLineInfo != null)
									{
										firstReferenceLineInfo = referenceLineInfo;
										firstReferenceImageZComponent = imageZComponent;
									}
								}

								// >= keeps the last image as close to the end of the display set as possible.
								if (imageZComponent >= lastReferenceImageZComponent)
								{
									ReferenceLineInfo? referenceLineInfo = GetReferenceLineInfo(frame, targetFrame);
									if (referenceLineInfo != null)
									{
										lastReferenceLineInfo = referenceLineInfo;
										lastReferenceImageZComponent = imageZComponent;
									}
								}
							}
						}
					}
				}
			}
		}

		private ReferenceLineInfo? GetCurrentReferenceLineInfo(IPresentationImage targetImage)
		{
			Frame currentReferenceFrame = ((IImageSopProvider) _currentReferenceImage).Frame;
			Frame targetFrame = ((IImageSopProvider) targetImage).Frame;

			return GetReferenceLineInfo(currentReferenceFrame, targetFrame);
		}

		private IEnumerable<ReferenceLineInfo> GetAllReferenceLineInfo(IPresentationImage targetImage)
		{
			if (_currentReferenceImage != null && targetImage != null && targetImage is IImageSopProvider &&
			    _currentReferenceImage.ParentDisplaySet.ImageBox != targetImage.ParentDisplaySet.ImageBox)
			{
				Frame currentReferenceFrame = ((IImageSopProvider) _currentReferenceImage).Frame;
				Frame targetFrame = ((IImageSopProvider) targetImage).Frame;

				if (targetFrame.FrameOfReferenceUid == currentReferenceFrame.FrameOfReferenceUid &&
				    targetFrame.ParentImageSop.StudyInstanceUID == currentReferenceFrame.ParentImageSop.StudyInstanceUID)
				{
					ImageInfo targetInfo = _cache.GetImageInformation(targetFrame);
					if (targetInfo != null)
					{
						ReferenceLineInfo? firstReferenceLineInfo, lastReferenceLineInfo;
						GetFirstAndLastReferenceLineInfo(targetImage, out firstReferenceLineInfo, out lastReferenceLineInfo);

						if (firstReferenceLineInfo != null && lastReferenceLineInfo != null)
						{
							yield return firstReferenceLineInfo.Value;
							yield return lastReferenceLineInfo.Value;
						}

						ReferenceLineInfo? currentReferenceLineInfo = GetCurrentReferenceLineInfo(targetImage);
						if (currentReferenceLineInfo != null)
							yield return currentReferenceLineInfo.Value;
					}
				}
			}
		}

		private bool RefreshTargetImage(IPresentationImage targetImage)
		{
			ReferenceLineCompositeGraphic referenceLineCompositeGraphic = GetReferenceLineCompositeGraphic(targetImage);
			if (referenceLineCompositeGraphic == null)
				return false;

			bool needsRedraw = false;

			if (!Active)
			{
				if (referenceLineCompositeGraphic.Visible)
				{
					referenceLineCompositeGraphic.Visible = false;
					needsRedraw = true;
				}
			}
			else 
			{
				if (!referenceLineCompositeGraphic.Visible)
				{
					referenceLineCompositeGraphic.Visible = true;
					needsRedraw = true;
				}

				if (referenceLineCompositeGraphic.Tag != _currentReferenceImageInfo)
				{
					needsRedraw = true;
					referenceLineCompositeGraphic.Tag = _currentReferenceImageInfo;

					int i = 0;
					foreach (ReferenceLineInfo info in GetAllReferenceLineInfo(targetImage))
					{
						ReferenceLineGraphic referenceLineGraphic = referenceLineCompositeGraphic[i++];
						referenceLineGraphic.Point1 = info.StartPoint;
						referenceLineGraphic.Point2 = info.EndPoint;
						referenceLineGraphic.Text = info.Label;
						referenceLineGraphic.Visible = true;
					}

					// make any that aren't valid invisible.
					for (int j = i; j < referenceLineCompositeGraphic.Graphics.Count; ++j)
					{
						if (referenceLineCompositeGraphic[j].Visible)
							referenceLineCompositeGraphic[j].Visible = false;
					}
				}
			}

			return needsRedraw;
		}

		public IEnumerable<IPresentationImage> GetRefreshedImages()
		{
			SetCurrentReferenceImage();

			// Calculation of the reference lines is deferred until image drawing.  This method
			// really only exists to determine which images should be drawn so that the coordinator/mediator
			// can efficiently draw only the images that need to be redrawn.
			foreach (IImageBox imageBox in this.Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				foreach (ITile tile in imageBox.Tiles)
				{
					IPresentationImage targetImage = tile.PresentationImage;

					if (targetImage != null && RefreshTargetImage(targetImage))
						yield return targetImage;
				}
			}
		}
	}
}
