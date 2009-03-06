using System;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	[MouseToolButton(XMouseButtons.Left, false)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuVolume/Define Oblique", "Apply", Flags = ClickActionFlags.CheckAction)]
	//[ButtonAction("activate", "global-toolbars/ToolbarsMpr/Define Oblique", "Apply", Flags = ClickActionFlags.CheckAction)]
	[IconSet("activate", IconScheme.Colour, "Icons.DefineObliqueToolLarge.png", "Icons.DefineObliqueToolMedium.png", "Icons.DefineObliqueToolSmall.png")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[VisibleStateObserver("activate", "Visible", "VisibleChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class DefineObliqueTool : MouseImageViewerTool
	{
		private MprImageViewerToolHelper _toolHelper;
		private StandardStatefulInteractiveGraphic _polyLine;
		private CompositeUndoableCommand _undoableCommand;
		private bool _visible;

		public DefineObliqueTool()
		{
			Behaviour |= MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		public event EventHandler VisibleChanged;

		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (_visible != value)
				{
					_visible = value;
					EventsHelper.Fire(VisibleChanged, this, EventArgs.Empty);
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolHelper = new MprImageViewerToolHelper(Context);

			Visible = IsValidImage(this.SelectedPresentationImage);
		}

		private void RemoveGraphic()
		{
			if (_polyLine != null)
			{
				IPresentationImage image = _polyLine.ParentPresentationImage;

				_undoableCommand.Unexecute();
				_undoableCommand = null;
				
				_polyLine.ControlPoints.ControlPointChangedEvent -= OnControlPointChanged;
				_polyLine.Drawing -= OnPolyLineDrawing;
				_polyLine.Dispose();
				_polyLine = null;
				image.Draw();
			}
		}

		private void OnActivationChanged(object sender, EventArgs e)
		{
			RemoveGraphic();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			RemoveGraphic();
			Visible = IsValidImage(this.SelectedPresentationImage);
		}

		// This hack works around the problem where the OnTileSelected gets called in
		//	situations where the DisplaySet has already been created. No need to recreate every time.
		//  Ideally there would be a better way for the tool to know when to Create the full set.
		private bool _fullSetCreated = false;
		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			MprDisplaySet displaySet = _toolHelper.GetObliqueDisplaySet();
			if (displaySet != null && _fullSetCreated == false)
			{
				displaySet.CreateFullObliqueDisplaySet();
				_fullSetCreated = true;
			}
			RemoveGraphic();
			Visible = IsValidImage(this.SelectedPresentationImage);
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (_polyLine != null)
				return _polyLine.Start(mouseInformation);

			IPresentationImage image = mouseInformation.Tile.PresentationImage;

			if (!IsValidImage(image))
				return false;

			IOverlayGraphicsProvider provider = image as IOverlayGraphicsProvider;
			if (provider == null)
				return false;

			_polyLine = new StandardStatefulInteractiveGraphic(new PolyLineInteractiveGraphic(2));
			_polyLine.InactiveColor = Color.CornflowerBlue;
			_polyLine.FocusColor = Color.Cyan;
			_polyLine.SelectedColor = Color.Blue;
			_polyLine.FocusSelectedColor = Color.Blue;

			_polyLine.State = new CreatePolyLineGraphicState(_polyLine);

			_undoableCommand = new CompositeUndoableCommand();
			_undoableCommand.Enqueue(new InsertGraphicUndoableCommand(_polyLine, provider.OverlayGraphics, provider.OverlayGraphics.Count));
			_undoableCommand.Execute();

			if (_polyLine.Start(mouseInformation))
			{
				_polyLine.ControlPoints.ControlPointChangedEvent += OnControlPointChanged;
				_polyLine.Drawing += OnPolyLineDrawing;
				base.ActivationChanged += OnActivationChanged;
				return true;
			}

			this.Cancel();
			return false;
		}

		//TODO: Ideally handling this event wouldn't be necessary. Perhaps a Move event would solve the issue.
		//	When the line is moved as a whole the OnControlPointChanged event is fired as the control points 
		//	are offset individually. This results in endpoint wierdness that makes the oblique viewport behave erraticly. 
		//	So here I grab the endpoints and use them in OnControlPointChange.
		private void OnPolyLineDrawing(object sender, EventArgs e)
		{
#if true
			_polyLine.CoordinateSystem = CoordinateSystem.Destination;

			PointF start = _polyLine.SpatialTransform.ConvertToSource(_polyLine.ControlPoints[0]);
			PointF end = _polyLine.SpatialTransform.ConvertToSource(_polyLine.ControlPoints[1]);

			_polyLine.ResetCoordinateSystem();

			_startPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(start);
			_endPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(end);
#endif
		}

		private Vector3D _startPatient;
		private Vector3D _endPatient;

		private void OnControlPointChanged(object sender, ListEventArgs<System.Drawing.PointF> e)
		{
#if false  // Code moved to OnPolyLineDrawing above, enable this to see the erratic behavior
			_polyLine.CoordinateSystem = CoordinateSystem.Destination;

			PointF start = _polyLine.SpatialTransform.ConvertToSource(_polyLine.ControlPoints[0]);
			PointF end = _polyLine.SpatialTransform.ConvertToSource(_polyLine.ControlPoints[1]);

			_polyLine.ResetCoordinateSystem();

			_startPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(start);
			_endPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(end);
#endif
			if (_startPatient == null || _endPatient == null)
				return;
	
			if ((_startPatient - _endPatient).Magnitude < 5 * base.SelectedImageSopProvider.Frame.NormalizedPixelSpacing.Row)
				return;

			Vector3D orientationRow = new Vector3D(
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.RowX,
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.RowY,
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.RowZ);

			Vector3D orientationColumn = new Vector3D(
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.ColumnX,
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.ColumnY,
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.ColumnZ);

			_toolHelper.GetObliqueDisplaySet().SetCutLine(orientationColumn, orientationRow, _startPatient, _endPatient);
			_fullSetCreated = false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_polyLine != null)
				return _polyLine.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_polyLine == null)
				return false;

			if (_polyLine.Stop(mouseInformation))
				return true;

			//RemoveGraphic();
			return false;
		}

		public override void Cancel()
		{
			if (_polyLine == null)
				return;

			_polyLine.Cancel();
			RemoveGraphic();
		}

		public void Apply()
		{
			if (!Visible)
				return;

			Select();
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_polyLine != null)
				return _polyLine.GetCursorToken(point);

			return null;
		}

		private bool IsValidImage(IPresentationImage image)
		{
			return _toolHelper.IsIdentityImage(image) 
				|| _toolHelper.IsOrthoXImage(image)
				|| _toolHelper.IsOrthoYImage(image);
		}
	}
}
