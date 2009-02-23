using System;
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
	[MenuAction("activate", "imageviewer-contextmenu/Define Oblique", "Apply", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarsMpr/Define Oblique", "Apply", Flags = ClickActionFlags.CheckAction)]
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
			base.Behaviour = MouseButtonHandlerBehaviour.SuppressOnTileActivate;
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

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
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
			_polyLine.InactiveColor = Color.MediumBlue;
			_polyLine.FocusColor = Color.LightBlue;
			_polyLine.SelectedColor = Color.Blue;
			_polyLine.FocusSelectedColor = Color.Blue;

			_polyLine.State = new CreatePolyLineGraphicState(_polyLine);

			_undoableCommand = new CompositeUndoableCommand();
			_undoableCommand.Enqueue(new InsertGraphicUndoableCommand(_polyLine, provider.OverlayGraphics, provider.OverlayGraphics.Count));
			_undoableCommand.Execute();

			if (_polyLine.Start(mouseInformation))
			{
				_polyLine.ControlPoints.ControlPointChangedEvent += OnControlPointChanged;
				base.ActivationChanged += new EventHandler(OnActivationChanged);
				return true;
			}

			this.Cancel();
			return false;
		}

		private void OnControlPointChanged(object sender, ListEventArgs<System.Drawing.PointF> e)
		{
			_polyLine.CoordinateSystem = CoordinateSystem.Destination;

			PointF start = _polyLine.SpatialTransform.ConvertToSource(_polyLine.ControlPoints[0]);
			PointF end = _polyLine.SpatialTransform.ConvertToSource(_polyLine.ControlPoints[1]);

			_polyLine.ResetCoordinateSystem();

			Vector3D startPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(start);
			Vector3D endPatient = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(end);

			Vector3D orientationRow = new Vector3D(
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.RowX,
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.RowY,
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.RowZ);

			Vector3D orientationColumn = new Vector3D(
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.ColumnX,
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.ColumnY,
				(float)SelectedImageSopProvider.Frame.ImageOrientationPatient.ColumnZ);

			_toolHelper.GetObliqueDisplaySet().SetCutLine(orientationColumn, orientationRow, startPatient, endPatient);
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

			RemoveGraphic();
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
			return _toolHelper.IsIdentityImage(image);
		}
	}
}
