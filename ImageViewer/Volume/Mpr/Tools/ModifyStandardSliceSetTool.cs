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
		[MouseToolButton(XMouseButtons.Left, false)]
		[MenuAction("activate", "imageviewer-contextmenu/MenuDefineSlicePlane", "Select", Flags = ClickActionFlags.CheckAction)]
		[ButtonAction("activate", "global-toolbars/ToolbarsMpr/MenuDefineSlicePlane", "Select", Flags = ClickActionFlags.CheckAction)]
		[IconSet("activate", IconScheme.Colour, "Icons.DefineObliqueToolLarge.png", "Icons.DefineObliqueToolMedium.png", "Icons.DefineObliqueToolSmall.png")]
		[CheckedStateObserver("activate", "Active", "ActivationChanged")]
		[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
		[LabelValueObserver("activate", "Label", "SliceSetChanged")]
		//TODO JY
		private class ModifyStandardSliceSetTool : MprViewerTool
		{
			private PolylineGraphic _polyLine;
			private InteractivePolylineGraphicBuilder _graphicBuilder;
			private CompositeUndoableCommand _undoableCommand;

			private IMprStandardSliceSet _sliceSet;

			public ModifyStandardSliceSetTool()
			{
				base.Behaviour |= MouseButtonHandlerBehaviour.SuppressOnTileActivate;
			}

			public event EventHandler SliceSetChanged;

			public IMprStandardSliceSet SliceSet
			{
				get { return _sliceSet; }
				set
				{
					if (_sliceSet != value)
					{
						_sliceSet = value;
						EventsHelper.Fire(this.SliceSetChanged, this, EventArgs.Empty);
					}
				}
			}

			public string Label
			{
				get
				{
					if (_sliceSet != null)
					{
						IImageBox imageBox = FindImageBox(_sliceSet, this.ImageViewer);
						if (imageBox != null)
							return string.Format(SR.MenuDefineSlicePlaneFor, imageBox.DisplaySet.Description);
					}
					return string.Empty;
				}
			}

			private void RemoveGraphic()
			{
				if (_polyLine != null)
				{
					IPresentationImage image = _polyLine.ParentPresentationImage;

					_undoableCommand.Unexecute();
					_undoableCommand = null;

					RemoveGraphicBuilder();

					_polyLine.Points.PointChanged -= OnAnchorPointChanged;
					_polyLine.Drawing -= OnPolyLineDrawing;
					_polyLine.Dispose();
					_polyLine = null;
					image.Draw();
				}
			}

			private void RemoveGraphicBuilder()
			{
				if (_graphicBuilder != null)
				{
					_graphicBuilder.GraphicComplete -= OnGraphicBuilderDone;
					_graphicBuilder.GraphicCancelled -= OnGraphicBuilderDone;
					_graphicBuilder = null;
				}
			}

			private void OnActivationChanged(object sender, EventArgs e)
			{
				//RemoveGraphic();
			}

			public override bool Start(IMouseInformation mouseInformation)
			{
				base.Start(mouseInformation);

				if (_graphicBuilder != null)
					return _graphicBuilder.Start(mouseInformation);

				IPresentationImage image = mouseInformation.Tile.PresentationImage;

				IOverlayGraphicsProvider provider = image as IOverlayGraphicsProvider;
				if (provider == null)
					return false;

				RemoveGraphic();

				_polyLine = new PolylineGraphic();

				StandardStatefulGraphic statefulPolyline = new StandardStatefulGraphic(new VerticesControlGraphic(new MoveControlGraphic(_polyLine)));
				statefulPolyline.InactiveColor = Color.CornflowerBlue;
				statefulPolyline.FocusColor = Color.Cyan;
				statefulPolyline.SelectedColor = Color.Blue;
				statefulPolyline.FocusSelectedColor = Color.Blue;
				statefulPolyline.State = statefulPolyline.CreateInactiveState();

				_undoableCommand = new CompositeUndoableCommand();
				_undoableCommand.Enqueue(new InsertGraphicUndoableCommand(statefulPolyline, provider.OverlayGraphics, provider.OverlayGraphics.Count));
				_undoableCommand.Execute();

				_graphicBuilder = new InteractivePolylineGraphicBuilder(2, _polyLine);
				_graphicBuilder.GraphicComplete += OnGraphicBuilderDone;
				_graphicBuilder.GraphicCancelled += OnGraphicBuilderDone;

				if (_graphicBuilder.Start(mouseInformation))
				{
					_polyLine.Points.PointChanged += OnAnchorPointChanged;
					_polyLine.Drawing += OnPolyLineDrawing;
					base.ActivationChanged += OnActivationChanged;
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
				_polyLine.CoordinateSystem = CoordinateSystem.Destination;

				PointF start = _polyLine.SpatialTransform.ConvertToSource(_polyLine.Points[0]);
				PointF end = _polyLine.SpatialTransform.ConvertToSource(_polyLine.Points[1]);

				_polyLine.ResetCoordinateSystem();

				_startPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(start);
				_endPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(end);
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

				Vector3D orientationRow = new Vector3D(
					(float) SelectedImageSopProvider.Frame.ImageOrientationPatient.RowX,
					(float) SelectedImageSopProvider.Frame.ImageOrientationPatient.RowY,
					(float) SelectedImageSopProvider.Frame.ImageOrientationPatient.RowZ);

				Vector3D orientationColumn = new Vector3D(
					(float) SelectedImageSopProvider.Frame.ImageOrientationPatient.ColumnX,
					(float) SelectedImageSopProvider.Frame.ImageOrientationPatient.ColumnY,
					(float) SelectedImageSopProvider.Frame.ImageOrientationPatient.ColumnZ);

				if (this.SliceSet != null && !this.SliceSet.IsReadOnly)
				{
					IImageBox imageBox =
						CollectionUtils.SelectFirst(base.ImageViewer.PhysicalWorkspace.ImageBoxes, delegate(IImageBox test) { return test.DisplaySet.Uid == this.SliceSet.Uid; });
					IDisplaySet displaySet = imageBox.DisplaySet;
					imageBox.DisplaySet = null;

					this.SliceSet.SlicerParams = VolumeSlicerParams.CreateSlicing(this.SliceSet.Volume, orientationColumn, orientationRow, _startPatient, _endPatient);

					imageBox.DisplaySet = displaySet;
					imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count/2;
					imageBox.DisplaySet.Draw();
				}
			}

			public override bool Track(IMouseInformation mouseInformation)
			{
				if (_graphicBuilder != null)
					return _graphicBuilder.Track(mouseInformation);

				return false;
			}

			public override bool Stop(IMouseInformation mouseInformation)
			{
				if (_graphicBuilder == null)
					return false;

				if (_graphicBuilder.Stop(mouseInformation))
				{
					return true;
				}

				//RemoveGraphic();
				return false;
			}

			public override void Cancel()
			{
				if (_graphicBuilder == null)
					return;

				_graphicBuilder.Cancel();
				RemoveGraphic();
			}

			public override CursorToken GetCursorToken(Point point)
			{
				if (_graphicBuilder != null)
					return _graphicBuilder.GetCursorToken(point);

				return base.GetCursorToken(point);
			}
		}
	}
}