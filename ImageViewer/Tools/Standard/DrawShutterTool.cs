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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using System;
using ClearCanvas.Common.Utilities;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal enum ShutterType
	{
		Polygon,
		Circle,
		Rectangle
	}

	#region Toolbar

	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarDrawShutter", "Select", "DrawShutterMenuModel", Flags = ClickActionFlags.CheckAction)]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[IconSetObserver("activate", "IconSet", "SelectedShutterTypeChanged")]

	#region Drop-down

	[ButtonAction("selectDrawCircleShutter", "drawshutter-toolbar-dropdown/MenuDrawCircleShutter", "SelectDrawCircleShutter")]
	[IconSet("selectDrawCircleShutter", IconScheme.Colour, "Icons.DrawCircularShutterToolSmall.png", "Icons.DrawCircularShutterToolMedium.png", "Icons.DrawCircularShutterToolLarge.png")]
	[CheckedStateObserver("selectDrawCircleShutter", "DrawCircleShutterChecked", "SelectedShutterTypeChanged")]

	[ButtonAction("selectDrawPolygonShutter", "drawshutter-toolbar-dropdown/MenuDrawPolygonShutter", "SelectDrawPolygonShutter")]
	[IconSet("selectDrawPolygonShutter", IconScheme.Colour, "Icons.DrawPolygonalShutterToolSmall.png", "Icons.DrawPolygonalShutterToolMedium.png", "Icons.DrawPolygonalShutterToolLarge.png")]
	[CheckedStateObserver("selectDrawPolygonShutter", "DrawPolygonShutterChecked", "SelectedShutterTypeChanged")]

	[ButtonAction("selectDrawRectangleShutter", "drawshutter-toolbar-dropdown/MenuDrawRectangleShutter", "SelectDrawRectangleShutter")]
	[IconSet("selectDrawRectangleShutter", IconScheme.Colour, "Icons.DrawRectangularShutterToolSmall.png", "Icons.DrawRectangularShutterToolMedium.png", "Icons.DrawRectangularShutterToolLarge.png")]
	[CheckedStateObserver("selectDrawRectangleShutter", "DrawRectangleShutterChecked", "SelectedShutterTypeChanged")]

	#endregion
	#endregion

	#region Menu

	[ButtonAction("selectDrawCircleShutter", "global-menus/MenuTools/MenuStandard/MenuDrawCircleShutter", "SelectDrawCircleShutter")]
	[GroupHint("selectDrawCircleShutter", "Tools.Image.Manipulation.Shutter")]
	[EnabledStateObserver("selectDrawCircleShutter", "Enabled", "EnabledChanged")]

	[ButtonAction("selectDrawPolygonShutter", "global-menus/MenuTools/MenuStandard/MenuDrawPolygonShutter", "SelectDrawPolygonShutter")]
	[GroupHint("selectDrawPolygonShutter", "Tools.Image.Manipulation.Shutter")]
	[EnabledStateObserver("selectDrawPolygonShutter", "Enabled", "EnabledChanged")]

	[ButtonAction("selectDrawRectangleShutter", "global-menus/MenuTools/MenuStandard/MenuDrawRectangleShutter", "SelectDrawRectangleShutter")]
	[GroupHint("selectDrawCircleShutter", "Tools.Image.Manipulation.Shutter")]
	[EnabledStateObserver("selectDrawRectangleShutter", "Enabled", "EnabledChanged")]

	#endregion

	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]

	public class DrawShutterTool : MouseImageViewerTool
	{
		private ShutterType _selectedShutterType;
		private InteractiveGraphicBuilder _graphicBuilder;
		private IGraphic _primitiveGraphic;

		public DrawShutterTool()
			: base(SR.TooltipDrawShutter)
		{
			_selectedShutterType = ShutterType.Polygon;
			this.Behaviour |= MouseButtonHandlerBehaviour.SuppressContextMenu | MouseButtonHandlerBehaviour.SuppressOnTileActivate | MouseButtonHandlerBehaviour.ConstrainToTile;
		}

		public bool DrawCircleShutterChecked
		{
			get { return _selectedShutterType == ShutterType.Circle; }
		}

		public bool DrawRectangleShutterChecked
		{
			get { return _selectedShutterType == ShutterType.Rectangle; }
		}

		public bool DrawPolygonShutterChecked
		{
			get { return _selectedShutterType == ShutterType.Polygon; }
		}

		private ShutterType SelectedShutterType
		{
			get { return _selectedShutterType; }
			set
			{
				if (_selectedShutterType == value)
					return;

				_selectedShutterType = value;
				EventsHelper.Fire(SelectedShutterTypeChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler SelectedShutterTypeChanged;

		public IconSet IconSet
		{
			get
			{
				if (_selectedShutterType == ShutterType.Rectangle)
					return new IconSet(IconScheme.Colour, "Icons.DrawRectangularShutterToolSmall.png", "Icons.DrawRectangularShutterToolMedium.png", "Icons.DrawRectangularShutterToolLarge.png");
				else if (_selectedShutterType == ShutterType.Polygon)
					return new IconSet(IconScheme.Colour, "Icons.DrawPolygonalShutterToolSmall.png", "Icons.DrawPolygonalShutterToolMedium.png", "Icons.DrawPolygonalShutterToolLarge.png");
				else
					return new IconSet(IconScheme.Colour, "Icons.DrawCircularShutterToolSmall.png", "Icons.DrawCircularShutterToolMedium.png", "Icons.DrawCircularShutterToolLarge.png");
			}	
		}

		public void SelectDrawRectangleShutter()
		{
			SelectedShutterType = ShutterType.Rectangle;
			base.Select();
		}

		public void SelectDrawPolygonShutter()
		{
			SelectedShutterType = ShutterType.Polygon;
			base.Select();
		}

		public void SelectDrawCircleShutter()
		{
			SelectedShutterType = ShutterType.Circle;
			base.Select();
		}
		
		public ActionModelNode DrawShutterMenuModel
		{
			get
			{
				return ActionModelRoot.CreateModel(typeof (DrawShutterTool).FullName, "drawshutter-toolbar-dropdown", base.Actions);
			}	
		}

		public override void Initialize()
		{
			base.Initialize();
			base.ImageViewer.EventBroker.ImageDrawing += OnImageDrawing;
		}

		protected override void Dispose(bool disposing)
			{
			base.ImageViewer.EventBroker.ImageDrawing -= OnImageDrawing;
			base.Dispose(disposing);
			}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			UpdateEnabled(e.SelectedTile.PresentationImage);
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			UpdateEnabled(e.SelectedPresentationImage);
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
			{
			if (e.PresentationImage.Selected)
				UpdateEnabled(e.PresentationImage);
		}

		private void UpdateEnabled(IPresentationImage image)
		{
			bool enabled = false;
			if (image != null && image is IDicomPresentationImage)
			{
				DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image as IDicomPresentationImage, false);
				if (dicomGraphicsPlane != null)
					enabled = dicomGraphicsPlane.Shutters.Enabled;
			}

			this.Enabled = enabled;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (!Enabled)
				return false;

			base.Start(mouseInformation);

			if (_graphicBuilder != null)
				return _graphicBuilder.Start(mouseInformation);

			IDicomPresentationImage image = mouseInformation.Tile.PresentationImage as IDicomPresentationImage;
			if (image == null || GetGeometricShuttersGraphic(image) == null)
				return false;

			AddDrawShutterGraphic(image);

			if (_graphicBuilder.Start(mouseInformation))
			{
				_primitiveGraphic.Draw();
				return true;
			}

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (!Enabled)
				return false;

			if (_graphicBuilder != null)
			{
				bool returnValue = _graphicBuilder.Track(mouseInformation);
				_primitiveGraphic.Draw();
				return returnValue;
			}

			return base.Track(mouseInformation);
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (!Enabled)
				return false;

			if (_graphicBuilder != null && _graphicBuilder.Stop(mouseInformation))
			{
				_primitiveGraphic.Draw();
				return true;
			}

			if (_graphicBuilder == null && _primitiveGraphic != null)
			{
				bool boundingBoxTooSmall = false;
				_primitiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				if (_primitiveGraphic.BoundingBox.Width < 50 || _primitiveGraphic.BoundingBox.Height < 50)
					boundingBoxTooSmall = true;
				_primitiveGraphic.ResetCoordinateSystem();

				if (boundingBoxTooSmall)
				{
					RemoveDrawShutterGraphic();
					base.SelectedPresentationImage.Draw();
				}
				else
				{
					GeometricShutter shutter = ConvertToGeometricShutter();
					GeometricShuttersGraphic shuttersGraphic =
						GetGeometricShuttersGraphic((IDicomPresentationImage) base.SelectedPresentationImage);
					DrawableUndoableCommand command = new DrawableUndoableCommand(shuttersGraphic);
					command.Name = SR.CommandDrawShutter;
					command.Enqueue(new AddGeometricShutterUndoableCommand(shuttersGraphic, shutter));
					command.Execute();

					base.ImageViewer.CommandHistory.AddCommand(command);
				}
			}

				return false;
		}

		public override void Cancel()
		{
			if (_graphicBuilder != null)
			{
				_graphicBuilder.Cancel();
				_graphicBuilder = null;
			}

			if (_primitiveGraphic != null)
			{
				IPresentationImage image = _primitiveGraphic.ParentPresentationImage;
				RemoveDrawShutterGraphic();
				image.Draw();
			}
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_graphicBuilder != null)
				return _graphicBuilder.GetCursorToken(point);

			return base.GetCursorToken(point);
		}

		private void AddDrawShutterGraphic(IDicomPresentationImage image)
		{
			switch (_selectedShutterType)
			{
				case ShutterType.Polygon:
					{
						_primitiveGraphic = new PolylineGraphic();
						image.OverlayGraphics.Add(_primitiveGraphic);
						_graphicBuilder = InteractiveShutterGraphicBuilders.CreatePolygonalShutterBuilder((PolylineGraphic)_primitiveGraphic);
						break;
					}
				case ShutterType.Circle:
					{
						_primitiveGraphic = new EllipsePrimitive();
						image.OverlayGraphics.Add(_primitiveGraphic);
						_graphicBuilder = InteractiveShutterGraphicBuilders.CreateCircularShutterBuilder((EllipsePrimitive)_primitiveGraphic);
						break;
					}
				default:
					{
						_primitiveGraphic = new RectanglePrimitive();
						image.OverlayGraphics.Add(_primitiveGraphic);
						_graphicBuilder = InteractiveShutterGraphicBuilders.CreateRectangularShutterBuilder((RectanglePrimitive)_primitiveGraphic);
						break;
					}
			}

			_graphicBuilder.GraphicCancelled += OnGraphicCancelled;
			_graphicBuilder.GraphicComplete += OnGraphicComplete;
		}

		private void OnGraphicComplete(object sender, GraphicEventArgs e)
		{
			_graphicBuilder.GraphicCancelled -= OnGraphicCancelled;
			_graphicBuilder.GraphicComplete -= OnGraphicComplete;
			_graphicBuilder = null;
		}

		private void OnGraphicCancelled(object sender, GraphicEventArgs e)
		{
			_graphicBuilder.GraphicCancelled -= OnGraphicCancelled;
			_graphicBuilder.GraphicComplete -= OnGraphicComplete;
			_graphicBuilder = null;
		}

		private void RemoveDrawShutterGraphic()
		{
			if (_primitiveGraphic != null)
			{
				((CompositeGraphic)_primitiveGraphic.ParentGraphic).Graphics.Remove(_primitiveGraphic);
				_primitiveGraphic.Dispose();
				_primitiveGraphic = null;
			}
		}

		private GeometricShutter ConvertToGeometricShutter()
		{
			GeometricShutter shutter;

			if (_selectedShutterType == ShutterType.Rectangle)
			{
				RectanglePrimitive primitive = (RectanglePrimitive)_primitiveGraphic;
				primitive.CoordinateSystem = CoordinateSystem.Source;
				Rectangle rectangle =
					new Rectangle((int)primitive.TopLeft.X, (int)primitive.TopLeft.Y, (int)primitive.Width, (int)primitive.Height);
				primitive.ResetCoordinateSystem();
				
				shutter = new RectangularShutter(rectangle);
			}
			else if (_selectedShutterType == ShutterType.Polygon)
			{
				PolylineGraphic polyLine = (PolylineGraphic)_primitiveGraphic;
				polyLine.CoordinateSystem = CoordinateSystem.Source;

				List<Point> points = new List<Point>();
				for (int i = 0; i < polyLine.Points.Count; ++i)
					points.Add(new Point((int)polyLine.Points[i].X, (int)polyLine.Points[i].Y));

				polyLine.ResetCoordinateSystem();
				shutter = new PolygonalShutter(points);
			}
			else
			{
				EllipsePrimitive primitive = (EllipsePrimitive)_primitiveGraphic;
				primitive.CoordinateSystem = CoordinateSystem.Source;
				Rectangle rectangle = new Rectangle((int)primitive.TopLeft.X, (int)primitive.TopLeft.Y, (int)primitive.Width, (int)primitive.Height);
				rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
				int radius = rectangle.Width/2;
				Point center = new Point(rectangle.X + radius, rectangle.Y + radius);
				primitive.ResetCoordinateSystem();

				shutter = new CircularShutter(center, radius);
			}

			RemoveDrawShutterGraphic();
			return shutter;
		}

		internal static IDicomGraphicsPlaneShutters GetShuttersGraphic(IDicomPresentationImage image)
		{
			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
			if (dicomGraphicsPlane != null)
				return dicomGraphicsPlane.Shutters;
			return null;
		}

		internal static GeometricShuttersGraphic GetGeometricShuttersGraphic(IDicomPresentationImage image)
		{
			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);

			if (dicomGraphicsPlane == null)
				return null;

			return CollectionUtils.SelectFirst(dicomGraphicsPlane.Shutters,
				delegate(IShutterGraphic shutter) { return shutter is GeometricShuttersGraphic; }) as GeometricShuttersGraphic;
		}
	}
}
