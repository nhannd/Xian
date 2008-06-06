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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
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
    public partial class SpatialLocatorTool : MouseImageViewerTool
	{
		#region Private Fields

		private SynchronizationToolCoordinator _coordinator;

		private DicomImagePlane _referencePlane;
		private readonly List<SpatialLocatorReferencePoint> _referencePoints;
		private bool _inUse;

		#endregion

		public SpatialLocatorTool()
			: base(SR.PrefixSpatialLocatorTool)
		{
			_inUse = false;
			_referencePoints = new List<SpatialLocatorReferencePoint>();

			this.CursorToken = new CursorToken("SpatialLocatorCursor.png", this.GetType().Assembly);
		}

		#region Tool Overrides

		public override void Initialize()
        {
            base.Initialize();
			
			_coordinator = SynchronizationToolCoordinator.Get(base.ImageViewer);
        	_coordinator.SpatialLocatorTool = this;
        }

		protected override void Dispose(bool disposing)
		{
			_coordinator.Release();

			base.Dispose(disposing);
		}

		#endregion

		#region Private Methods

		private void GetPlaneClosestToReferenceImagePoint(PointF referenceImagePoint,
							IEnumerable<DicomImagePlane> targetImagePlanes,
							out DicomImagePlane closestTargetImagePlane,
							out PointF closestTargetImagePoint)
		{
			closestTargetImagePlane = null;
			closestTargetImagePoint = PointF.Empty;

			float distanceToClosestImagePlane = float.MaxValue;

			Vector3D referencePositionPatient = _referencePlane.ConvertToPatient(referenceImagePoint);

			foreach (DicomImagePlane targetImagePlane in targetImagePlanes)
			{
				float halfThickness = Math.Abs(targetImagePlane.Thickness / 2);
				float halfSpacing = Math.Abs(targetImagePlane.Spacing / 2);
				float toleranceDistanceToImagePlane = Math.Max(halfThickness, halfSpacing);

				if (_referencePlane.IsInSameFrameOfReference(targetImagePlane))
				{
					if (toleranceDistanceToImagePlane > 0)
					{
						Vector3D positionTargetImagePlane = targetImagePlane.ConvertToImagePlane(referencePositionPatient);
						float distanceToTargetImagePlane = Math.Abs(positionTargetImagePlane.Z);

						if (distanceToTargetImagePlane <= toleranceDistanceToImagePlane && distanceToTargetImagePlane < distanceToClosestImagePlane)
						{
							distanceToClosestImagePlane = distanceToTargetImagePlane;
							//The coordinates need to be converted to pixel coordinates because right now they are in mm.
							closestTargetImagePoint = targetImagePlane.ConvertToImage(new PointF(positionTargetImagePlane.X, positionTargetImagePlane.Y));
							closestTargetImagePlane = targetImagePlane;
						}
					}
				}
			}
		}

		private SpatialLocatorReferencePoint GetReferencePoint(IImageBox imageBox)
		{
			SpatialLocatorReferencePoint referencePoint = _referencePoints.Find(
				delegate(SpatialLocatorReferencePoint test)
					{
						return test.ImageBox == imageBox;
					});

			if (referencePoint == null)
			{
				referencePoint = new SpatialLocatorReferencePoint(imageBox, this);
				_referencePoints.Add(referencePoint);
			}

			return referencePoint;
		}

		private IEnumerable<DicomImagePlane> GetTargetImagePlanes(IImageBox imageBox)
		{
			for (int i = imageBox.DisplaySet.PresentationImages.Count - 1; i >= 0; --i)
			{
				DicomImagePlane targetImagePlane = DicomImagePlane.FromImage(imageBox.DisplaySet.PresentationImages[i]);
				if (targetImagePlane != null && _referencePlane.IsInSameFrameOfReference(targetImagePlane))
					yield return targetImagePlane;
			}
		}

		private IEnumerable<IImageBox> GetTargetImageBoxes()
		{
			foreach (IImageBox imageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox.DisplaySet != null && !IsReferenceImageBox(imageBox))
					yield return imageBox;
			}
		}

		private bool IsReferenceImageBox(IImageBox imageBox)
		{
			return imageBox == _referencePlane.SourceImage.ParentDisplaySet.ImageBox;
		}

		private void UpdateReferencePoint(SpatialLocatorReferencePoint referencePoint, PointF referenceImagePoint)
		{
			DicomImagePlane closestTargetPlane;
			PointF closestTargetImagePoint;
			GetPlaneClosestToReferenceImagePoint(referenceImagePoint, GetTargetImagePlanes(referencePoint.ImageBox),
												out closestTargetPlane, out closestTargetImagePoint);

			if (closestTargetPlane == null)
			{
				referencePoint.Image = null;
			}
			else 
			{
				referencePoint.Image = closestTargetPlane.SourceImage;
				referencePoint.ImagePoint = closestTargetImagePoint;
			}
		}

		private void UpdateAllReferencePoints(Point destinationPoint)
		{
			PointF referenceImagePoint = _referencePlane.SourceImageTransform.ConvertToSource(destinationPoint);

			foreach (IImageBox imageBox in GetTargetImageBoxes())
				UpdateReferencePoint(GetReferencePoint(imageBox), referenceImagePoint);

			_coordinator.OnSpatialLocatorReferencePointsUpdated();
		}

		#endregion

		#region Mouse Handler Methods

		private bool Start()
		{
			_referencePlane = DicomImagePlane.FromImage(base.SelectedPresentationImage);
			return _referencePlane != null;
		}

		private void Stop()
		{
			_referencePlane = null;
			_inUse = false;

			foreach (SpatialLocatorReferencePoint referencePoint in _referencePoints)
				referencePoint.Image = null;

			_coordinator.OnSpatialLocatorStopped();

			foreach (SpatialLocatorReferencePoint referencePoint in _referencePoints)
				referencePoint.Dispose();

			_referencePoints.Clear();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			_inUse = Start();
			if (_inUse)
				UpdateAllReferencePoints(mouseInformation.Location);

			return _inUse;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_inUse)
				UpdateAllReferencePoints(mouseInformation.Location);

			return _inUse;
        }

		public override bool Stop(IMouseInformation mouseInformation)
		{
			Cancel();
			return false;
        }

		public override void Cancel()
		{
			Stop();
		}

		#endregion

		#region Internal Methods (for mediator)

		internal IEnumerable<IImageBox> GetImageBoxesToRedraw()
		{
			foreach (SpatialLocatorReferencePoint referencePoint in _referencePoints)
			{
				if (referencePoint.Dirty)
					yield return referencePoint.ImageBox;
			}
		}

		#endregion
	}
}
