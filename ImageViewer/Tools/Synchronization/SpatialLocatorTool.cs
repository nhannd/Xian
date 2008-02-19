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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[MenuAction("activate", "global-menus/MenuTools/MenuSynchronization/MenuSpatialLocator", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarSynchronization/ToolbarSpatialLocator", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.SpatialLocatorToolSmall.png", "Icons.SpatialLocatorToolMedium.png", "Icons.SpatialLocatorToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Synchronization.SpatialLocator")]
	
	[MouseToolButton(XMouseButtons.Right, false)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class SpatialLocatorTool : MouseImageViewerTool
    {
		private SynchronizationToolCoordinator _coordinator;
		private SopInfoCache _cache;

		private bool _inUse;

		public SpatialLocatorTool()
			: base(SR.PrefixSpatialLocatorTool)
		{
			_inUse = false;
			this.CursorToken = new CursorToken("SpatialLocatorCursor.png", this.GetType().Assembly);
        }

        public override void Initialize()
        {
            base.Initialize();
			
			_coordinator = SynchronizationToolCoordinator.Get(base.ImageViewer);
        	_coordinator.SpatialLocatorTool = this;

        	_cache = SopInfoCache.Get();
        }

		protected override void Dispose(bool disposing)
		{
			_coordinator.Release();
			_cache.Release();

			base.Dispose(disposing);
		}

		private SpatialLocatorGraphic GetSpatialLocatorGraphic(IPresentationImage image)
		{
			return _coordinator.GetSpatialLocatorGraphic(image);
		}

		private bool CalculateReferencePoint(IImageBox imageBox, Vector3D referencePositionPatient)
		{
			ImageSop referenceSop = base.SelectedImageSopProvider.ImageSop;

			int closestIndex = -1;
			float closestDistanceMillimetres = float.MaxValue;
			PointF closestPointImage = PointF.Empty;

			for (int i = imageBox.DisplaySet.PresentationImages.Count - 1; i >= 0; --i)
			{
				IPresentationImage image = imageBox.DisplaySet.PresentationImages[i];
				if (image is IImageSopProvider)
				{
					ImageSop sop = ((IImageSopProvider)image).ImageSop;

					if (referenceSop.StudyInstanceUID == sop.StudyInstanceUID && referenceSop.FrameOfReferenceUid == sop.FrameOfReferenceUid)
					{
						ImageInfo info = _cache.GetImageInformation(sop);
						if (info != null)
						{
							Vector3D positionImage = sop.ImagePlaneHelper.ConvertToImage(referencePositionPatient, info.PositionPatientTopLeft);

							float zDistanceMillimetres = Math.Abs(positionImage.Z);

							//The coordinates need to be converted to pixel coordinates because right now they are in mm.
							PointF positionImagePixels = (PointF)sop.ImagePlaneHelper.ConvertToImagePixel(new PointF(positionImage.X, positionImage.Y));

							if (zDistanceMillimetres < closestDistanceMillimetres)
							{
								closestIndex = i;
								closestDistanceMillimetres = zDistanceMillimetres;
								closestPointImage = positionImagePixels;
							}
						}
					}
				}
			}

			if (closestIndex >= 0)
			{
				imageBox.TopLeftPresentationImageIndex = closestIndex;

				SpatialLocatorGraphic existingGraphic = GetSpatialLocatorGraphic(imageBox.TopLeftPresentationImage);
				if (existingGraphic != null)
				{
					existingGraphic.CoordinateSystem = CoordinateSystem.Source;
					existingGraphic.Anchor = closestPointImage;
					existingGraphic.ResetCoordinateSystem();
					return true;
				}
			}

			return false;
		}

		private IEnumerable<IImageBox> CalculateReferencePoints(Vector3D referencePositionPatient)
		{
			foreach (IImageBox imageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox != this.SelectedPresentationImage.ParentDisplaySet.ImageBox)
				{
					if (imageBox.DisplaySet != null && CalculateReferencePoint(imageBox, referencePositionPatient))
						yield return imageBox;
				}
			}
		}
		
		private bool CalculateReferencePoints(Point destinationPoint)
		{
			if (base.SelectedImageSopProvider == null || base.SelectedSpatialTransformProvider == null ||
				!(base.SelectedSpatialTransformProvider.SpatialTransform is SpatialTransform))
				return false;

			ImageSop referenceSop = base.SelectedImageSopProvider.ImageSop;
			if (String.IsNullOrEmpty(referenceSop.FrameOfReferenceUid) || String.IsNullOrEmpty(referenceSop.StudyInstanceUID))
				return false;

			PointF sourcePoint = ((SpatialTransform)base.SelectedSpatialTransformProvider.SpatialTransform).ConvertToSource(destinationPoint);
			Vector3D referencePositionPatient = referenceSop.ImagePlaneHelper.ConvertToPatient(sourcePoint);
			if (referencePositionPatient == null)
				return false;

			_coordinator.OnSpatialLocatorPointsCalculated(CalculateReferencePoints(referencePositionPatient));
			return true;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			return (_inUse = CalculateReferencePoints(mouseInformation.Location));
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_inUse)
				CalculateReferencePoints(mouseInformation.Location);

			return _inUse;
        }

		public override bool Stop(IMouseInformation mouseInformation)
		{
			Cancel();
			return false;
        }

		public override void Cancel()
		{
			_coordinator.OnSpatialLocatorStopped();
			_inUse = false;
		}
	}
}
