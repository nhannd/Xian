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
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	partial class ResliceToolGroup
	{
		[MenuAction("activate", "imageviewer-contextmenu/MenuReslice", "Select", Flags = ClickActionFlags.CheckAction)]
		[MenuAction("activate", "global-menus/MenuTools/MenuMpr/MenuReslice", "Select", Flags = ClickActionFlags.CheckAction)]
		[IconSet("activate", IconScheme.Colour, "Icons.ResliceToolLarge.png", "Icons.ResliceToolMedium.png", "Icons.ResliceToolSmall.png")]
		[CheckedStateObserver("activate", "Active", "ActivationChanged")]
		[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
		[LabelValueObserver("activate", "Label", "SliceSetChanged")]
		[GroupHint("activate", "Tools.Volume.MPR.Reslicing")]
		[MouseToolButton(XMouseButtons.Left, false)]
		private class ResliceTool : MprViewerTool
		{
			private ResliceToolGraphic _resliceGraphic;
			private InteractivePolylineGraphicBuilder _lineGraphicBuilder;

			private object _graphicBuilderMemento;
			private TranslocateGraphicUndoableCommand _graphicTranslocationCommand;

			private Color _hotColor = Color.SkyBlue;
			private Color _normalColor = Color.CornflowerBlue;

			private int _lastTopLeftPresentationImageIndex = -1;

			public ResliceTool()
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
							return string.Format(SR.MenuResliceFor, this.SliceImageBox.DisplaySet.Description);
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

			private void OnImageViewerImageBoxDrawing(object sender, ImageBoxDrawingEventArgs e)
			{
				IImageBox imageBox = this.SliceImageBox;

				IDisplaySet containingDisplaySet = null;
				if (_resliceGraphic.ParentPresentationImage != null)
					containingDisplaySet = _resliceGraphic.ParentPresentationImage.ParentDisplaySet;

				if (containingDisplaySet != null && containingDisplaySet.ImageBox == e.ImageBox)
				{
					// translocate the graphic if the user is stacking through the display set that the graphic sits in
					// do not add this command to history - the stack command generates the actual action command
					if (_lastTopLeftPresentationImageIndex != e.ImageBox.TopLeftPresentationImageIndex)
					{
						_lastTopLeftPresentationImageIndex = e.ImageBox.TopLeftPresentationImageIndex;
						TranslocateGraphic(_resliceGraphic, e.ImageBox.TopLeftPresentationImage);
					}
				}
				else if (imageBox != null && imageBox == e.ImageBox)
				{
					// we're stacking on the set we control, so make sure the colourised display set name is replicated
					IPresentationImage firstReslicedImage = imageBox.TopLeftPresentationImage;
					ColorizeDisplaySetDescription(firstReslicedImage, this.NormalColor);

					// and realign the slice line with the stacked position
					if (_resliceGraphic.ParentPresentationImage != null && _resliceGraphic.ParentPresentationImage != firstReslicedImage)
					{
						_resliceGraphic.SetLine(imageBox.TopLeftPresentationImage, _resliceGraphic.ParentPresentationImage);
						_resliceGraphic.Draw();
					}
				}
			}

			protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
			{
				base.OnPresentationImageSelected(sender, e);

				IImageBox imageBox = this.SliceImageBox;

				IDisplaySet selectedDisplaySet = null;
				if (this.SelectedPresentationImage != null)
					selectedDisplaySet = this.SelectedPresentationImage.ParentDisplaySet;

				// only allow tool if we're in a MprViewerComponent, and we're not going to be operating on ourself!
				base.Enabled = (this.ImageViewer != null) &&
							   (imageBox != null && selectedDisplaySet != null && selectedDisplaySet != imageBox.DisplaySet);
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

			/// <summary>
			/// Gets the <see cref="IImageBox"/> containing the slicing we control.
			/// </summary>
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
					_lineGraphicBuilder.GraphicCancelled -= OnGraphicBuilderCancelled;
					_lineGraphicBuilder = null;
				}
			}

			public override void Initialize()
			{
				base.Initialize();

				if (this.SliceImageBox == null)
					throw new InvalidOperationException("Tool has nothing to control because the specified slice set is not visible.");

				_resliceGraphic = new ResliceToolGraphic();
				_resliceGraphic.Color = this.NormalColor;
				_resliceGraphic.HotColor = this.HotColor;
				_resliceGraphic.Points.PointChanged += OnAnchorPointChanged;
				_resliceGraphic.Text = this.SliceImageBox.DisplaySet.Description;

				// draw the reslice graphic on the first imagebox that isn't showing the slicing this tool controls and is not parallel
				foreach (IImageBox imageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
				{
					if (imageBox != this.SliceImageBox)
					{
						if (_resliceGraphic.SetLine(this.SliceImageBox.TopLeftPresentationImage, imageBox.TopLeftPresentationImage))
						{
							TranslocateGraphic(_resliceGraphic, imageBox.TopLeftPresentationImage);
							break;
						}
					}
				}
				ColorizeDisplaySetDescription(this.SliceImageBox.TopLeftPresentationImage, this.NormalColor);

				this.ImageViewer.EventBroker.ImageBoxDrawing += OnImageViewerImageBoxDrawing;
			}

			protected override void Dispose(bool disposing)
			{
				this.ImageViewer.EventBroker.ImageBoxDrawing -= OnImageViewerImageBoxDrawing;

				if (disposing)
				{
					if (this.SliceSet != null)
					{
						this.SliceSet = null;
					}

					if (_resliceGraphic != null)
					{
						TranslocateGraphic(_resliceGraphic, null);
						_resliceGraphic.Points.PointChanged -= OnAnchorPointChanged;
						_resliceGraphic.Dispose();
						_resliceGraphic = null;
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

				_graphicBuilderMemento = _resliceGraphic.CreateMemento();

				// if we are reslicing on a different (i.e. not the one it previously existed on) display set, set this field variable
				// which will be consumed when the graphic builder is completed or cancelled
				_graphicTranslocationCommand = TranslocateGraphic(_resliceGraphic, this.SelectedPresentationImage);

				// The interactive graphic builders typically operate on new, pristine graphics
				// Since our graphic isn't new, clear the points from it! (Otherwise you'll end up with a polyline)
				_resliceGraphic.Points.Clear();

				_lineGraphicBuilder = new InteractivePolylineGraphicBuilder(2, _resliceGraphic);
				_lineGraphicBuilder.GraphicComplete += OnGraphicBuilderDone;
				_lineGraphicBuilder.GraphicCancelled += OnGraphicBuilderCancelled;

				if (_lineGraphicBuilder.Start(mouseInformation))
				{
					return true;
				}

				this.Cancel();
				return false;
			}

			private void OnGraphicBuilderDone(object sender, GraphicEventArgs e)
			{
				if (base.ImageViewer.CommandHistory != null)
				{
					DrawableUndoableCommand compositeCommand;

					// if we had to translocate the graphic to start the builder, we will need to draw both the old and new parent images
					if (_graphicTranslocationCommand != null)
					{
						// disable automatic drawing by the translocation command
						_graphicTranslocationCommand.DrawOnExecuteUnexecute = false;

						// add the translocation command to the composite, and make that responsible for drawing
						compositeCommand = new DrawableUndoableCommand(new ResliceDrawable(_graphicTranslocationCommand.Drawable, this.Reslice));
					}
					else
					{
						// otherwise, we can get away with just drawing the graphic
						compositeCommand = new DrawableUndoableCommand(new ResliceDrawable(_resliceGraphic, this.Reslice));
					}

					// enqueue the translocate command
					if (_graphicTranslocationCommand != null)
						compositeCommand.Enqueue(_graphicTranslocationCommand);

					// enqueue the memento command
					MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(_resliceGraphic);
					memorableCommand.BeginState = _graphicBuilderMemento;
					memorableCommand.EndState = _resliceGraphic.CreateMemento();
					compositeCommand.Enqueue(memorableCommand);

					base.ImageViewer.CommandHistory.AddCommand(compositeCommand);
				}

				_graphicBuilderMemento = null;
				_graphicTranslocationCommand = null;

				RemoveGraphicBuilder();

				_lastTopLeftPresentationImageIndex = this.SliceImageBox.TopLeftPresentationImageIndex;
			}

			private class ResliceDrawable : IDrawable
			{
				public delegate IDrawable ResliceCommandDelegate();

				private readonly IDrawable _drawable;
				private readonly ResliceCommandDelegate _resliceCommand;

				public ResliceDrawable(IDrawable drawable1, ResliceCommandDelegate drawable2)
				{
					_drawable = drawable1;
					_resliceCommand = drawable2;
				}

				public void Draw()
				{
					// reslicing takes the longest, so run it first
					IDrawable drawable2 = _resliceCommand();
					if (drawable2 != null)
						drawable2.Draw();
					if (_drawable != null)
						_drawable.Draw();
				}

				public event EventHandler Drawing
				{
					add { }
					remove { }
				}
			}

			private void OnGraphicBuilderCancelled(object sender, GraphicEventArgs e)
			{
				IDrawable affectedDrawables;

				// if we had to translocate the graphic to start the builder, undo that now
				if (_graphicTranslocationCommand != null)
				{
					affectedDrawables = _graphicTranslocationCommand.Drawable;
					_graphicTranslocationCommand.DrawOnExecuteUnexecute = false;
					_graphicTranslocationCommand.Unexecute();
					_graphicTranslocationCommand = null;
				}
				else
				{
					affectedDrawables = _resliceGraphic;
				}

				if (_graphicBuilderMemento != null)
				{
					_resliceGraphic.SetMemento(_graphicBuilderMemento);
					_graphicBuilderMemento = null;
					IDrawable resliceResult = this.Reslice();
					if (resliceResult != null)
						resliceResult.Draw();

					affectedDrawables.Draw();
				}


				RemoveGraphicBuilder();
			}

			private void OnAnchorPointChanged(object sender, IndexEventArgs e)
			{
				IDrawable reslicedResult = this.Reslice();

				if (reslicedResult != null)
					reslicedResult.Draw();
			}

			private IDrawable Reslice()
			{
				Vector3D _startPatient = null;
				Vector3D _endPatient = null;

				IImageSopProvider imageSopProvider = _resliceGraphic.ParentPresentationImage as IImageSopProvider;

				// there must be two points already...
				if (_resliceGraphic.Points.Count > 1 && imageSopProvider != null)
				{
					_resliceGraphic.CoordinateSystem = CoordinateSystem.Destination;

					PointF start = _resliceGraphic.SpatialTransform.ConvertToSource(_resliceGraphic.Points[0]);
					PointF end = _resliceGraphic.SpatialTransform.ConvertToSource(_resliceGraphic.Points[1]);

					_resliceGraphic.ResetCoordinateSystem();

					_startPatient = imageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(start);
					_endPatient = imageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(end);
				}

				if (_startPatient == null || _endPatient == null)
					return null;

				if ((_startPatient - _endPatient).Magnitude < 5*imageSopProvider.Frame.NormalizedPixelSpacing.Row)
					return null;

				// set the new slice plane, which will regenerate the corresponding display set
				SetSlicePlane(this.SliceSet, _resliceGraphic.ParentPresentationImage, _startPatient, _endPatient);

				ColorizeDisplaySetDescription(this.SliceImageBox.TopLeftPresentationImage, this.NormalColor);

				return this.SliceImageBox.TopLeftPresentationImage;
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

			/// <summary>
			/// Gets the image on which the slice line is defined.
			/// </summary>
			public IPresentationImage ReferenceImage
			{
				get { return _resliceGraphic.ParentPresentationImage; }
			}

			/// <summary>
			/// Sets the image on which the slice line is defined.
			/// </summary>
			/// <param name="referenceImage"></param>
			/// <returns>True if the reference image was successfully changed; False otherwise (e.g. the specified image does not intersect the sliced images)</returns>
			public bool SetReferenceImage(IPresentationImage referenceImage)
			{
				if (_resliceGraphic.ParentPresentationImage != referenceImage)
				{
					if (referenceImage == null)
					{
						// if we change the reference image to nothing, hide the graphic
						TranslocateGraphic(_resliceGraphic, null);
						return true;
					}
					else if (_resliceGraphic.SetLine(this.SliceImageBox.TopLeftPresentationImage, referenceImage))
					{
						// if we change the reference image to something and we know how they intersect, move the graphic over
						TranslocateGraphic(_resliceGraphic, referenceImage);
						return true;
					}
					else
					{
						// if we change the reference image to something and they don't actually intersect (it happens in odd cases), hide the graphic but report failure
						TranslocateGraphic(_resliceGraphic, null);
						return false;
					}
				}
				return false;
			}
		}
	}
}