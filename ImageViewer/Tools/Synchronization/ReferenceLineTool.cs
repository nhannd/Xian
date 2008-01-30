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
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuReferenceLines", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarReferenceLines", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActiveChanged")]
	[Tooltip("activate", "TooltipReferenceLines")]
	[IconSet("activate", IconScheme.Colour, "Icons.CurrentReferenceLineToolSmall.png", "Icons.CurrentReferenceLineToolMedium.png", "Icons.CurrentReferenceLineToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Synchronization.ReferenceLines")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ReferenceLineTool : ImageViewerTool
	{
		#region ImageInfo struct

		private class ImageInfo
		{
			public Matrix RotationMatrix;
			public Vector3D Normal;
			public Vector3D PositionPatientTopLeft;
			public Vector3D PositionPatientBottomRight;
			public Vector3D PositionImagePlaneTopLeft;
		}

		#endregion

		private ImageInfo _currentReferenceImageInfo;
		private IPresentationImage _currentReferenceImage;
		private IPresentationImage _firstReferenceImage;
		private IPresentationImage _lastReferenceImage;
		
		private bool _active;
		private event EventHandler _activeChanged;

		private SynchronizationToolCoordinator _coordinator;

		private readonly Dictionary<string, ImageInfo> _sopInfoDictionary;

		private static readonly string _compositeGraphicName = "ReferenceLines";

		private const float _toleranceInRadians = (float)(1 * Math.PI / 180);

		public ReferenceLineTool()
		{
			_active = false;
			_sopInfoDictionary = new Dictionary<string, ImageInfo>();
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

			_coordinator = SynchronizationToolCoordinator.Get(base.ImageViewer);
			_coordinator.ReferenceLineTool = this;
		}

		protected override void Dispose(bool disposing)
		{
			base.ImageViewer.EventBroker.ImageDrawing -= OnImageDrawing;

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
				CompositeGraphic compositeGraphic = GetReferenceLineCompositeGraphic(e.PresentationImage);
				if (compositeGraphic != null)
					compositeGraphic.Visible = false;
			}
		}

		public void Toggle()
		{
			Active = !Active;
			_coordinator.OnReferenceLinesCalculated(CalculateReferenceLines());
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
					_currentReferenceImageInfo = GetImageInformation(((IImageSopProvider) _currentReferenceImage).ImageSop);
					valid = true;
				}
			}

			if (!valid)
			{
				_currentReferenceImage = null;
				_currentReferenceImageInfo = null;
			}

			CalculateFirstAndLastReferenceImages();
		}

		private ImageInfo GetImageInformation(ImageSop sop)
		{
			ImageInfo info;

			if (!_sopInfoDictionary.ContainsKey(sop.SopInstanceUID))
			{
				info = new ImageInfo();
				info.PositionPatientTopLeft = ImagePositionHelper.SourceToPatientTopLeftOfImage(sop);
				info.PositionPatientBottomRight = ImagePositionHelper.SourceToPatientBottomRightOfImage(sop);
				info.RotationMatrix = ImagePositionHelper.GetRotationMatrix(sop);

				if (info.PositionPatientTopLeft == null || info.PositionPatientBottomRight == null || info.RotationMatrix == null)
					return null;

				info.Normal = new Vector3D(info.RotationMatrix[2, 0], info.RotationMatrix[2, 1], info.RotationMatrix[2, 2]);

				// Transform the position (patient) vector to the coordinate system of the image.
				// This way, the z-components will all be along the same vector path.
				Matrix positionPatientMatrix = new Matrix(3, 1);
				positionPatientMatrix.SetColumn(0, info.PositionPatientTopLeft.X, info.PositionPatientTopLeft.Y, info.PositionPatientTopLeft.Z);
				Matrix result = info.RotationMatrix * positionPatientMatrix;

				info.PositionImagePlaneTopLeft = new Vector3D(result[0, 0], result[1, 0], result[2, 0]);

				_sopInfoDictionary[sop.SopInstanceUID] = info;
			}
			else
			{
				info = _sopInfoDictionary[sop.SopInstanceUID];
			}

			return info;
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
					ImageInfo info = GetImageInformation(sop);
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

		private static CompositeGraphic GetReferenceLineCompositeGraphic(IPresentationImage image)
		{
			if (image == null || !(image is INamedCompositeGraphicProvider))
				return null;

			return ((INamedCompositeGraphicProvider) image).GetNamedCompositeGraphic(_compositeGraphicName);
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
						ImageInfo info = GetImageInformation(sop);
						if (info != null)
						{
							// 2. Is the image within 1 degree of being in the same plane as the current image?
							float angle = Math.Abs((float)Math.Acos(info.Normal.Dot(_currentReferenceImageInfo.Normal)));
							if (angle <= _toleranceInRadians || (Math.PI - angle) <= _toleranceInRadians)
							{
								// 3. Use the Image Position (in the coordinate system of the Image Plane!) to determine the 
								//    first and last reference line.  By transforming the Image Position (Patient) to 
								//    the coordinate system of the image plane, we can then simply take the 2 images with
								//    the smallest and largest z-components, respectively, as the 'first' and 'last' reference images.
								float imageZComponent = info.PositionImagePlaneTopLeft.Z;

								if (imageZComponent < firstReferenceImageZComponent)
								{
									_firstReferenceImage = image;
									firstReferenceImageZComponent = imageZComponent;
								}
								if (imageZComponent > lastReferenceImageZComponent)
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
			ImageInfo referenceImageInfo = GetImageInformation(referenceImageSop);
			ImageInfo projectImageInfo = GetImageInformation(projectImageSop);

			// Transform the reference image diagonal to the destination image's coordinate system.
			Matrix topLeftPatient = new Matrix(3, 1);
			topLeftPatient.SetColumn(0, 
										referenceImageInfo.PositionPatientTopLeft.X - projectImageInfo.PositionPatientTopLeft.X,
										referenceImageInfo.PositionPatientTopLeft.Y - projectImageInfo.PositionPatientTopLeft.Y,
										referenceImageInfo.PositionPatientTopLeft.Z - projectImageInfo.PositionPatientTopLeft.Z);

			Matrix bottomRightPatient = new Matrix(3, 1);
			bottomRightPatient.SetColumn(0,
										referenceImageInfo.PositionPatientBottomRight.X - projectImageInfo.PositionPatientTopLeft.X,
										referenceImageInfo.PositionPatientBottomRight.Y - projectImageInfo.PositionPatientTopLeft.Y,
										referenceImageInfo.PositionPatientBottomRight.Z - projectImageInfo.PositionPatientTopLeft.Z);

			topLeftPatient = projectImageInfo.RotationMatrix * topLeftPatient;
			bottomRightPatient = projectImageInfo.RotationMatrix * bottomRightPatient;

			PixelSpacing spacing = projectImageSop.PixelSpacing;
			float spacingRow = (float)spacing.Row;
			float spacingColumn = (float)spacing.Column;

			//The coordinates need to be converted to pixel coordinates b/c right now they are in mm.
			topLeft = new PointF(topLeftPatient[0, 0] / spacingColumn, topLeftPatient[1, 0] / spacingRow);
			bottomRight = new PointF(bottomRightPatient[0, 0] / spacingColumn, bottomRightPatient[1, 0] / spacingRow);
		}

		private void GetReferenceLineInfo(IPresentationImage referenceImage, IPresentationImage image, out string text, out PointF topLeft, out PointF bottomRight)
		{
			ImageSop referenceSop = ((IImageSopProvider) referenceImage).ImageSop;
			TransformPoints(referenceSop, ((IImageSopProvider)image).ImageSop, out topLeft, out bottomRight);
			
			//TODO: later, add a config option to show slice location.
			text = referenceSop.InstanceNumber.ToString();
		}

		private void CreateReferenceLines(CompositeGraphic referenceLineCompositeGraphic, int number)
		{
			for(int index = referenceLineCompositeGraphic.Graphics.Count; index < number; ++index)
				referenceLineCompositeGraphic.Graphics.Add(new ReferenceLineGraphic());
		}

		private void SetReferenceLineInfo(CompositeGraphic referenceLineCompositeGraphic, int index, string text, PointF point1, PointF point2)
		{
			ReferenceLineGraphic referenceLine = (ReferenceLineGraphic)referenceLineCompositeGraphic.Graphics[index];
			referenceLine.CoordinateSystem = CoordinateSystem.Source;
			referenceLine.Point1 = point1;
			referenceLine.Point2 = point2;
			referenceLine.Text = text;	
			referenceLine.ResetCoordinateSystem();
		}

		public void CalculateReferenceLines(IPresentationImage image)
		{
			string text;
			PointF point1, point2;

			CompositeGraphic referenceLineCompositeGraphic = GetReferenceLineCompositeGraphic(image);
			CreateReferenceLines(referenceLineCompositeGraphic, 3);

			GetReferenceLineInfo(_currentReferenceImage, image, out text, out point1, out point2);
			SetReferenceLineInfo(referenceLineCompositeGraphic, 0, text, point1, point2);

			GetReferenceLineInfo(_firstReferenceImage, image, out text, out point1, out point2);
			SetReferenceLineInfo(referenceLineCompositeGraphic, 1, text, point1, point2);

			GetReferenceLineInfo(_lastReferenceImage, image, out text, out point1, out point2);
			SetReferenceLineInfo(referenceLineCompositeGraphic, 2, text, point1, point2);

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
						CompositeGraphic compositeGraphic = GetReferenceLineCompositeGraphic(image);
						if (compositeGraphic != null && compositeGraphic.Visible)
							yield return image;
					}
				}
			}
		}
	}
}
