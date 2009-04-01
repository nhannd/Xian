#if DEBUG

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
using ClearCanvas.ImageViewer.DicomGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal enum ShutterType
	{
		Polygon,
		Circle,
		Rectangle
	}

	#region Menu

	[MenuAction("selectDrawCircleShutter", "global-menus/MenuTools/MenuStandard/MenuDrawCircleShutter", "SelectDrawCircleShutter")]
	[CheckedStateObserver("selectDrawCircleShutter", "DrawCircleShutterChecked", "SelectedShutterTypeChanged")]

	[MenuAction("selectDrawPolygonShutter", "global-menus/MenuTools/MenuStandard/MenuDrawPolygonShutter", "SelectDrawPolygonShutter")]
	[CheckedStateObserver("selectDrawPolygonShutter", "DrawPolygonShutterChecked", "SelectedShutterTypeChanged")]

	[MenuAction("selectDrawRectangleShutter", "global-menus/MenuTools/MenuStandard/MenuDrawRectangleShutter", "SelectDrawRectangleShutter")]
	[CheckedStateObserver("selectDrawRectangleShutter", "DrawRectangleShutterChecked", "SelectedShutterTypeChanged")]
	
	#endregion
	#region Toolbar

	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarDrawShutter", "Select", "DrawShutterMenuModel", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuDrawShutter", "Select", Flags = ClickActionFlags.CheckAction)]
	[GroupHint("activate", "Tools.Image.Manipulation.Shutter")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[IconSetObserver("activate", "IconSet", "SelectedShutterTypeChanged")]

	#region Drop-down

	[ButtonAction("selectDrawCircleShutter", "drawshutter-toolbar-dropdown/MenuDrawCircleShutter", "SelectDrawCircleShutter")]
	[IconSet("selectDrawCircleShutter", IconScheme.Colour, "Icons.DrawCircleShutterToolSmall.png", "Icons.DrawCircleShutterToolMedium.png", "Icons.DrawCircleShutterToolLarge.png")]
	[CheckedStateObserver("selectDrawCircleShutter", "DrawCircleShutterChecked", "SelectedShutterTypeChanged")]

	[ButtonAction("selectDrawPolygonShutter", "drawshutter-toolbar-dropdown/MenuDrawPolygonShutter", "SelectDrawPolygonShutter")]
	[IconSet("selectDrawPolygonShutter", IconScheme.Colour, "Icons.DrawPolygonShutterToolSmall.png", "Icons.DrawPolygonShutterToolMedium.png", "Icons.DrawPolygonShutterToolLarge.png")]
	[CheckedStateObserver("selectDrawPolygonShutter", "DrawPolygonShutterChecked", "SelectedShutterTypeChanged")]

	[ButtonAction("selectDrawRectangleShutter", "drawshutter-toolbar-dropdown/MenuDrawRectangleShutter", "SelectDrawRectangleShutter")]
	[IconSet("selectDrawRectangleShutter", IconScheme.Colour, "Icons.DrawRectangleShutterToolSmall.png", "Icons.DrawRectangleShutterToolMedium.png", "Icons.DrawRectangleShutterToolLarge.png")]
	[CheckedStateObserver("selectDrawRectangleShutter", "DrawRectangleShutterChecked", "SelectedShutterTypeChanged")]

	#endregion
	#endregion

	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class DrawShutterTool : MouseImageViewerTool
	{
		#region MouseInformation Proxy

		private class MouseInformationProxy : IMouseInformation
		{
			private readonly IMouseInformation _mouseInformation;
			private readonly Point _constrainedPoint;

			public MouseInformationProxy(IMouseInformation mouseInformation, Point constrainedPoint)
			{
				_mouseInformation = mouseInformation;
				_constrainedPoint = constrainedPoint;
			}

			#region IMouseInformation Members

			public ITile Tile
			{
				get { return _mouseInformation.Tile; }
			}

			public Point Location
			{
				get { return _constrainedPoint; }
			}

			public XMouseButtons ActiveButton
			{
				get { return _mouseInformation.ActiveButton; }
			}

			public uint ClickCount
			{
				get { return _mouseInformation.ClickCount; }
			}

			#endregion
		}

		#endregion

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
					return new IconSet(IconScheme.Colour, "Icons.DrawRectangleShutterToolSmall.png", "Icons.DrawRectangleShutterToolMedium.png", "Icons.DrawRectangleShutterToolLarge.png");
				else if (_selectedShutterType == ShutterType.Polygon)
					return new IconSet(IconScheme.Colour, "Icons.DrawPolygonShutterToolSmall.png", "Icons.DrawPolygonShutterToolMedium.png", "Icons.DrawPolygonShutterToolLarge.png");
				else
					return new IconSet(IconScheme.Colour, "Icons.DrawCircleShutterToolSmall.png", "Icons.DrawCircleShutterToolMedium.png", "Icons.DrawCircleShutterToolLarge.png");
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
				ShuttersGraphic shutters = GetShuttersGraphic(image as IDicomPresentationImage);
				if (shutters != null)
					enabled = shutters.Visible;
			}

			this.Enabled = enabled;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
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
			if (_graphicBuilder != null)
			{
				Point constrained = ConstrainMouseLocation(mouseInformation.Location);
				bool returnValue = _graphicBuilder.Track(new MouseInformationProxy(mouseInformation, constrained));
				_primitiveGraphic.Draw();
				return returnValue;
			}

			return base.Track(mouseInformation);
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
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
						_primitiveGraphic = new PolyLineGraphic();
						image.OverlayGraphics.Add(_primitiveGraphic);
						_graphicBuilder = new InteractivePolygonGraphicBuilder((PolyLineGraphic)_primitiveGraphic);
						break;
					}
				case ShutterType.Circle:
					{
						_primitiveGraphic = new EllipsePrimitive();
						image.OverlayGraphics.Add(_primitiveGraphic);
						_graphicBuilder = new InteractiveBoundableGraphicBuilder((EllipsePrimitive)_primitiveGraphic);
						break;
					}
				default:
					{
						_primitiveGraphic = new RectanglePrimitive();
						image.OverlayGraphics.Add(_primitiveGraphic);
						_graphicBuilder = new InteractiveBoundableGraphicBuilder((RectanglePrimitive)_primitiveGraphic);
						break;
					}
			}

			((IVectorGraphic)_primitiveGraphic).Color = Color.LightSteelBlue;
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

		private Point ConstrainMouseLocation(Point realMouseLocation)
		{
			Point constrained = realMouseLocation;

			//Constrain to circle
			if (_primitiveGraphic is EllipsePrimitive)
			{
				_primitiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				EllipsePrimitive ellipse = (EllipsePrimitive)_primitiveGraphic;
				Vector3D widthVector = new Vector3D(realMouseLocation.X - ellipse.TopLeft.X, 0, 0);
				Vector3D heightVector = new Vector3D(0, realMouseLocation.Y - ellipse.TopLeft.Y, 0);

				if (!Vector3D.AreEqual(widthVector, heightVector) && widthVector.Magnitude > 1 && heightVector.Magnitude > 1)
				{
					if (widthVector.Magnitude < heightVector.Magnitude)
					{
						heightVector = heightVector * widthVector.Magnitude / heightVector.Magnitude;
						constrained = new Point(constrained.X, (int)(ellipse.TopLeft.Y + heightVector.Y));
					}
					else
					{
						widthVector = widthVector * heightVector.Magnitude / widthVector.Magnitude;
						constrained = new Point((int)(ellipse.TopLeft.X + widthVector.X), constrained.Y);
					}
				}

				_primitiveGraphic.ResetCoordinateSystem();
			}

			return constrained;
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
				PolyLineGraphic polyLine = (PolyLineGraphic)_primitiveGraphic;
				polyLine.CoordinateSystem = CoordinateSystem.Source;

				List<Point> points = new List<Point>();
				for (int i = 0; i < polyLine.Count; ++i)
					points.Add(new Point((int)polyLine[i].X, (int)polyLine[i].Y));

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

		internal static ShuttersGraphic GetShuttersGraphic(IDicomPresentationImage image)
		{
			return CollectionUtils.SelectFirst(image.DicomGraphics,
							delegate(IGraphic graphic) { return graphic is ShuttersGraphic; }) as ShuttersGraphic;
		}

		internal static GeometricShuttersGraphic GetGeometricShuttersGraphic(IDicomPresentationImage image)
		{
			ShuttersGraphic parent = CollectionUtils.SelectFirst(image.DicomGraphics,
							delegate(IGraphic graphic) { return graphic is ShuttersGraphic; }) as ShuttersGraphic;

			if (parent == null)
				return null;

			return parent.GeometricShutters;
		}
	}
}

#endif