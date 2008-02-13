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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

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
		private ImageInfo _currentReferenceImageInfo;
		private IPresentationImage _currentReferenceImage;
		private IPresentationImage _firstReferenceImage;
		private IPresentationImage _lastReferenceImage;
		
		private bool _active;
		private event EventHandler _activeChanged;

		private SynchronizationToolCoordinator _coordinator;
		private SopInfoCache _cache;

		private static readonly float _toleranceInRadians = (float)(1 * Math.PI / 180);

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
			if (ShouldShowReferenceLinesForImage(e.PresentationImage))
			{
				CalculateReferenceLines(e.PresentationImage);
			}
			else
			{
				ReferenceLineCompositeGraphic compositeGraphic = GetReferenceLineCompositeGraphic(e.PresentationImage);
				if (compositeGraphic != null)
					compositeGraphic.Visible = false;
			}
		}

		public void Toggle()
		{
			Active = !Active;
			_coordinator.OnReferenceLinesCalculated(CalculateReferenceLines());
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
				ImageSop currentReferenceSop = ((IImageSopProvider) _currentReferenceImage).ImageSop;
				if (!String.IsNullOrEmpty(currentReferenceSop.FrameOfReferenceUid) && !String.IsNullOrEmpty(currentReferenceSop.StudyInstanceUID))
				{
					_currentReferenceImageInfo = _cache.GetImageInformation(((IImageSopProvider)_currentReferenceImage).ImageSop);
					valid = _currentReferenceImageInfo != null;
				}
			}

			if (!valid)
			{
				_currentReferenceImage = null;
				_currentReferenceImageInfo = null;
			}

			CalculateFirstAndLastReferenceImages();
		}

		private bool ShouldShowReferenceLinesForImage(IPresentationImage image)
		{
			if (!Active || _currentReferenceImage == null)
				return false;

			if (image == null || _currentReferenceImage.ParentDisplaySet.ImageBox == image.ParentDisplaySet.ImageBox)
				return false;
			
			if (GetReferenceLineCompositeGraphic(image) == null)
				return false;

			if (image is IImageSopProvider)
			{
				ImageSop currentReferenceSop = ((IImageSopProvider)_currentReferenceImage).ImageSop;
				ImageSop sop = ((IImageSopProvider)image).ImageSop;

				if (sop.FrameOfReferenceUid == currentReferenceSop.FrameOfReferenceUid && sop.StudyInstanceUID == currentReferenceSop.StudyInstanceUID)
				{
					ImageInfo info = _cache.GetImageInformation(sop);
					if (info != null)
					{
						float ninetyMinusAngle = (float)Math.Abs(Math.PI / 2 - Math.Abs(Math.Acos(info.Normal.Dot(_currentReferenceImageInfo.Normal))));
						if (ninetyMinusAngle <= _toleranceInRadians)
							return true;
					}
				}
			}

			return false;
		}

		private void CalculateFirstAndLastReferenceImages()
		{
			_firstReferenceImage = _lastReferenceImage = null;

			if (_currentReferenceImage == null)
				return;

			ImageSop currentReferenceSop = ((IImageSopProvider)_currentReferenceImage).ImageSop;

			_firstReferenceImage = _lastReferenceImage = _currentReferenceImage;

			float firstReferenceImageZComponent = float.MaxValue;
			float lastReferenceImageZComponent = float.MinValue;

			// 1. Find all images in the same plane as the current reference image.
			foreach (IPresentationImage image in _currentReferenceImage.ParentDisplaySet.PresentationImages)
			{
				if (image is IImageSopProvider)
				{
					ImageSop sop = ((IImageSopProvider)image).ImageSop;
					if (sop.FrameOfReferenceUid == currentReferenceSop.FrameOfReferenceUid && sop.StudyInstanceUID == currentReferenceSop.StudyInstanceUID)
					{
						ImageInfo info = _cache.GetImageInformation(sop);
						if (info != null)
						{
							// 2. Is the image within 1 degree of being in the same plane as the current image?
							float angle = Math.Abs((float)Math.Acos(info.Normal.Dot(_currentReferenceImageInfo.Normal)));
							if (angle <= _toleranceInRadians || (Math.PI - angle) <= _toleranceInRadians)
							{
								// 3. Use the Image Position (in the coordinate system of the Image Plane without moving the origin!) 
								//    to determine the first and last reference line.  By transforming the Image Position (Patient) to 
								//    the coordinate system of the image plane, we can then simply take the 2 images with
								//    the smallest and largest z-components, respectively, as the 'first' and 'last' reference images.
								float imageZComponent = info.PositionImagePlaneTopLeft.Z;

								// < keeps the first image as close to the beginning of the display set as possible.
								if (imageZComponent < firstReferenceImageZComponent)
								{
									_firstReferenceImage = image;
									firstReferenceImageZComponent = imageZComponent;
								}
								
								// >= keeps the last image as close to the end of the display set as possible.
								if (imageZComponent >= lastReferenceImageZComponent)
								{
									_lastReferenceImage = image;
									lastReferenceImageZComponent = imageZComponent;
								}
							}
						}
					}
				}
			}
		}

		private void TransformPoints(ImageSop referenceImageSop, ImageSop projectImageSop, out PointF topLeft, out PointF bottomRight)
		{
			ImageInfo referenceImageInfo = _cache.GetImageInformation(referenceImageSop);
			ImageInfo projectImageInfo = _cache.GetImageInformation(projectImageSop);

			// Transform the reference image diagonal to the destination image's coordinate system (pixel position 0,0 as the origin).
			Vector3D transformedTopLeft = projectImageSop.ConvertToImage(referenceImageInfo.PositionPatientTopLeft, projectImageInfo.PositionPatientTopLeft);
			Vector3D transformedBottomRight = projectImageSop.ConvertToImage(referenceImageInfo.PositionPatientBottomRight, projectImageInfo.PositionPatientTopLeft);

			//The coordinates need to be converted to pixel coordinates because right now they are in mm.
			topLeft = (PointF)projectImageSop.ConvertToImagePixel(new PointF(transformedTopLeft.X, transformedTopLeft.Y));
			bottomRight = (PointF)projectImageSop.ConvertToImagePixel(new PointF(transformedBottomRight.X, transformedBottomRight.Y));
		}

		private void CalculateReferenceLine
			(
				IPresentationImage referenceImage, 
				IPresentationImage image, 
				ReferenceLineCompositeGraphic referenceLineCompositeGraphic, 
				int index
			)
		{
			PointF topLeft, bottomRight;
			ImageSop referenceSop = ((IImageSopProvider)referenceImage).ImageSop;
			TransformPoints(referenceSop, ((IImageSopProvider)image).ImageSop, out topLeft, out bottomRight);

			//TODO: later, add a config option to show slice location.
			string text = referenceSop.InstanceNumber.ToString();

			ReferenceLineGraphic referenceLine = referenceLineCompositeGraphic[index];
			referenceLine.CoordinateSystem = CoordinateSystem.Source;
			referenceLine.Point1 = topLeft;
			referenceLine.Point2 = bottomRight;
			referenceLine.Text = text;	
			referenceLine.ResetCoordinateSystem();
		}

		public void CalculateReferenceLines(IPresentationImage image)
		{
			ReferenceLineCompositeGraphic referenceLineCompositeGraphic = GetReferenceLineCompositeGraphic(image);
			
			CalculateReferenceLine(_currentReferenceImage, image, referenceLineCompositeGraphic, 0);
			CalculateReferenceLine(_firstReferenceImage, image, referenceLineCompositeGraphic, 1);
			CalculateReferenceLine(_lastReferenceImage, image, referenceLineCompositeGraphic, 2);

			referenceLineCompositeGraphic.Visible = true;
		}

		public IEnumerable<IPresentationImage> CalculateReferenceLines()
		{
			SetCurrentReferenceImage();

			// Calculation of the reference lines is deferred until image drawing.  This method
			// really only exists to determine which images should be drawn so that the coordinator/mediator
			// can efficiently draw only the images that need to be redrawn.
			foreach (IImageBox imageBox in this.Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				foreach (ITile tile in imageBox.Tiles)
				{
					IPresentationImage image = tile.PresentationImage;
					if (ShouldShowReferenceLinesForImage(image))
					{
						yield return image;
					}
					else
					{
						ReferenceLineCompositeGraphic compositeGraphic = GetReferenceLineCompositeGraphic(image);
						if (compositeGraphic != null && compositeGraphic.Visible)
							yield return image;
					}
				}
			}
		}
	}
}
