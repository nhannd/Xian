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

	[GroupHint("activate", "Tools.Mpr.Creation")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class DefineObliqueTool : MouseImageViewerTool
	{
		private MprImageViewerToolHelper _toolHelper;
		private PolyLineInteractiveGraphic _polyLine;
		private CompositeUndoableCommand _undoableCommand;

		public DefineObliqueTool()
		{
			base.Behaviour = MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		private bool _visible;
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

			Visible = _toolHelper.GetMprLayoutManager() != null;
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
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			RemoveGraphic();
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

			_polyLine = new PolyLineInteractiveGraphic(true, 2);
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

			Vector3D normal = base.SelectedImageSopProvider.Frame.ImagePlaneHelper.GetNormalVector();

			_toolHelper.GetMprLayoutManager().SetObliqueCutLine(_toolHelper.GetObliqueImage(), normal, startPatient, endPatient);
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

		private bool IsValidImage(IPresentationImage image)
		{
			return image != null && (
			          	_toolHelper.IsAxialImage(image) ||
			          	_toolHelper.IsCoronalImage(image) ||
			          	_toolHelper.IsSaggittalImage(image));
		}
	}
}
