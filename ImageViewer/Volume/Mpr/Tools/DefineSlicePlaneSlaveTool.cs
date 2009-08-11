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
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	partial class DefineSlicePlaneTool
	{
		[MenuAction("activate", "imageviewer-contextmenu/MenuDefineSlicePlane", "Select", Flags = ClickActionFlags.CheckAction)]
		[ButtonAction("activate", "global-toolbars/ToolbarsMpr/MenuDefineSlicePlane", "Select", Flags = ClickActionFlags.CheckAction)]
		[IconSet("activate", IconScheme.Colour, "Icons.DefineObliqueToolLarge.png", "Icons.DefineObliqueToolMedium.png", "Icons.DefineObliqueToolSmall.png")]
		[CheckedStateObserver("activate", "Active", "ActivationChanged")]
		[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
		[LabelValueObserver("activate", "Label", "SliceSetChanged")]
		[MouseToolButton(XMouseButtons.Left, false)]
		private class DefineSlicePlaneSlaveTool : MprViewerTool
		{
			private SliceLineGraphic _lineGraphic;
			private InteractivePolylineGraphicBuilder _lineGraphicBuilder;

			private Color _hotColor = Color.SkyBlue;
			private Color _normalColor = Color.CornflowerBlue;

			public DefineSlicePlaneSlaveTool()
			{
				base.Behaviour |= MouseButtonHandlerBehaviour.SuppressOnTileActivate;
			}

			public string Label
			{
				get
				{
					if (_sliceSet != null)
					{
						if (this.SliceImageBox != null)
							return string.Format(SR.MenuDefineSlicePlaneFor, this.SliceImageBox.DisplaySet.Description);
					}
					return string.Empty;
				}
			}

			public Color HotColor
			{
				get { return _hotColor; }
				set { _hotColor = value; }
			}

			public Color NormalColor
			{
				get { return _normalColor; }
				set { _normalColor = value; }
			}

			protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
			{
				base.OnPresentationImageSelected(sender, e);

				IImageBox imageBox = this.SliceImageBox;

				// only allow tool if we're in a MprViewerComponent, and we're not going to be operating on ourself!
				base.Enabled = (this.ImageViewer != null) &&
				               (imageBox != null && this.SelectedPresentationImage != null && this.SelectedPresentationImage.ParentDisplaySet != imageBox.DisplaySet);

				// only translocate the graphic if the user is stacking through the display set
				if (_lineGraphic.ParentPresentationImage.ParentDisplaySet == this.SelectedPresentationImage.ParentDisplaySet)
					TranslocateGraphic(_lineGraphic, this.SelectedPresentationImage);

				ColorizeDisplaySetDescription(this.SliceImageBox.TopLeftPresentationImage, this.NormalColor);
			}

			#region Controlled SliceSet

			public event EventHandler SliceSetChanged;

			private IMprStandardSliceSet _sliceSet;
			private IImageBox _sliceImageBox;

			public IMprStandardSliceSet SliceSet
			{
				get { return _sliceSet; }
				set
				{
					if (_sliceSet != value)
					{
						_sliceSet = value;
						this.OnSliceSetChanged();
					}
				}
			}

			public IImageBox SliceImageBox
			{
				get
				{
					if (_sliceImageBox == null)
					{
						_sliceImageBox = FindImageBox(this.SliceSet, this.ImageViewer);
					}
					return _sliceImageBox;
				}
			}

			protected virtual void OnSliceSetChanged()
			{
				_sliceImageBox = null;
				EventsHelper.Fire(this.SliceSetChanged, this, EventArgs.Empty);
			}

			#endregion

			private void RemoveGraphicBuilder()
			{
				if (_lineGraphicBuilder != null)
				{
					_lineGraphicBuilder.GraphicComplete -= OnGraphicBuilderDone;
					_lineGraphicBuilder.GraphicCancelled -= OnGraphicBuilderDone;
					_lineGraphicBuilder = null;
				}
			}

			public override void Initialize()
			{
				base.Initialize();

				if (this.SliceImageBox == null)
					throw new InvalidOperationException("Tool has nothing to control because the specified slice set is not visible.");

				_lineGraphic = SliceLineGraphic.CreateSliceLineGraphic(this.ImageViewer, this.SliceSet, this.HotColor, this.NormalColor);
				_lineGraphic.LineGraphic.Points.PointChanged += OnAnchorPointChanged;
				_lineGraphic.Drawing += OnPolyLineDrawing;
				_lineGraphic.Text = this.SliceImageBox.DisplaySet.Description;

				foreach (IImageBox imageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
				{
					if (imageBox != this.SliceImageBox)
					{
						_lineGraphic.SetLine(this.SliceImageBox.TopLeftPresentationImage, imageBox.TopLeftPresentationImage);
						AddGraphic(_lineGraphic, imageBox.TopLeftPresentationImage);

						break;
					}
				}
				ColorizeDisplaySetDescription(this.SliceImageBox.TopLeftPresentationImage, this.NormalColor);
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (this.SliceSet != null)
					{
						this.SliceSet = null;
					}

					if (_lineGraphic != null)
					{
						RemoveGraphic(_lineGraphic);
						_lineGraphic.LineGraphic.Points.PointChanged -= OnAnchorPointChanged;
						_lineGraphic.Drawing -= OnPolyLineDrawing;
						_lineGraphic.Dispose();
						_lineGraphic = null;
					}
				}

				base.Dispose(disposing);
			}

			public override bool Start(IMouseInformation mouseInformation)
			{
				// don't let the tool start if we're disabled!
				if (!this.Enabled)
					return false;

				base.Start(mouseInformation);

				if (_lineGraphicBuilder != null)
					return _lineGraphicBuilder.Start(mouseInformation);

				IPresentationImage image = mouseInformation.Tile.PresentationImage;

				IOverlayGraphicsProvider provider = image as IOverlayGraphicsProvider;
				if (provider == null)
					return false;

				TranslocateGraphic(_lineGraphic, this.SelectedPresentationImage);

				// The interactive graphic builders typically operate on new, pristine graphics
				// Since our graphic isn't new, clear the points from it! (Otherwise you'll end up with a polyline)
				_lineGraphic.LineGraphic.Points.Clear();

				_lineGraphicBuilder = new InteractivePolylineGraphicBuilder(2, _lineGraphic.LineGraphic);
				_lineGraphicBuilder.GraphicComplete += OnGraphicBuilderDone;
				_lineGraphicBuilder.GraphicCancelled += OnGraphicBuilderDone;

				if (_lineGraphicBuilder.Start(mouseInformation))
				{
					return true;
				}

				this.Cancel();
				return false;
			}

			private void OnGraphicBuilderDone(object sender, GraphicEventArgs e)
			{
				RemoveGraphicBuilder();
			}

			//TODO: Ideally handling this event wouldn't be necessary. Perhaps a Move event would solve the issue.
			//	When the line is moved as a whole the OnAnchorPointChanged event is fired as the control points 
			//	are offset individually. This results in endpoint wierdness that makes the oblique viewport behave erraticly. 
			//	So here I grab the endpoints and use them in OnAnchorPointChanged.
			private void OnPolyLineDrawing(object sender, EventArgs e)
			{
#if true
				// there must be two points already...
				if (_lineGraphic.LineGraphic.Points.Count > 1 && base.SelectedImageSopProvider != null)
				{
					_lineGraphic.CoordinateSystem = CoordinateSystem.Destination;

					PointF start = _lineGraphic.SpatialTransform.ConvertToSource(_lineGraphic.LineGraphic.Points[0]);
					PointF end = _lineGraphic.SpatialTransform.ConvertToSource(_lineGraphic.LineGraphic.Points[1]);

					_lineGraphic.ResetCoordinateSystem();

					_startPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(start);
					_endPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(end);
				}
#endif
			}

			private Vector3D _startPatient;
			private Vector3D _endPatient;

			private void OnAnchorPointChanged(object sender, IndexEventArgs e)
			{
#if false // Code moved to OnPolyLineDrawing above, enable this to see the erratic behavior
			_polyLine.CoordinateSystem = CoordinateSystem.Destination;

			PointF start = _polyLine.SpatialTransform.ConvertToSource(_polyLine.ControlPoints[0]);
			PointF end = _polyLine.SpatialTransform.ConvertToSource(_polyLine.ControlPoints[1]);

			_polyLine.ResetCoordinateSystem();

			_startPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(start);
			_endPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(end);
#endif
				if (_startPatient == null || _endPatient == null)
					return;

				if ((_startPatient - _endPatient).Magnitude < 5*base.SelectedImageSopProvider.Frame.NormalizedPixelSpacing.Row)
					return;

				// set the new slice plane, which will regenerate the corresponding display set
				SetSlicePlane(this.SliceSet, this.SelectedPresentationImage, _startPatient, _endPatient);

				ColorizeDisplaySetDescription(this.SliceImageBox.TopLeftPresentationImage, this.NormalColor);

				this.SliceImageBox.TopLeftPresentationImage.Draw();
			}

			public override bool Track(IMouseInformation mouseInformation)
			{
				if (_lineGraphicBuilder != null)
					return _lineGraphicBuilder.Track(mouseInformation);

				return false;
			}

			public override bool Stop(IMouseInformation mouseInformation)
			{
				if (_lineGraphicBuilder == null)
					return false;

				if (_lineGraphicBuilder.Stop(mouseInformation))
				{
					return true;
				}

				return false;
			}

			public override void Cancel()
			{
				if (_lineGraphicBuilder == null)
					return;

				_lineGraphicBuilder.Cancel();
			}

			public override CursorToken GetCursorToken(Point point)
			{
				if (_lineGraphicBuilder != null)
					return _lineGraphicBuilder.GetCursorToken(point);

				return base.GetCursorToken(point);
			}
		}
	}
}