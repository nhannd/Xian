using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuRuler", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuRuler", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarRuler", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
	[Tooltip("activate", "TooltipRuler")]
	[IconSet("activate", IconScheme.Colour, "Icons.RulerToolSmall.png", "Icons.RulerToolMedium.png", "Icons.RulerToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Measurement.ROI.Linear")]

	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RulerTool : MouseImageViewerTool
	{
		private static readonly string[] _disallowedModalities = { "CR", "DX", "MG" };
		private ROIGraphic _roiGraphic;

		public RulerTool()
		{
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			IOverlayGraphicsProvider image = mouseInformation.Tile.PresentationImage as IOverlayGraphicsProvider;

			if (image == null)
				return false;

			if (_roiGraphic != null)
				return _roiGraphic.Start(mouseInformation);

			PolyLineInteractiveGraphic polyLineGraphic = new PolyLineInteractiveGraphic(true, 2);
			_roiGraphic = new ROIGraphic(polyLineGraphic, true);

			//_roiGraphic.Callout.Text = SR.ToolsMeasurementLineROI;
			image.OverlayGraphics.Add(_roiGraphic);
			_roiGraphic.RoiChanged += new EventHandler(OnRoiChanged);
			_roiGraphic.StateChanged += new EventHandler<GraphicStateChangedEventArgs>(OnRulerStateChanged);

			if (_roiGraphic.Start(mouseInformation))
				return true;

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_roiGraphic != null)
				return _roiGraphic.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_roiGraphic != null)
			{
				if (_roiGraphic.Stop(mouseInformation))
					return true;
			}

			_roiGraphic = null;
			return false;
		}

		public override void Cancel()
		{
			if (_roiGraphic != null)
				_roiGraphic.Cancel();

			IOverlayGraphicsProvider image = _roiGraphic.ParentPresentationImage as IOverlayGraphicsProvider;
			image.OverlayGraphics.Remove(_roiGraphic);

			_roiGraphic = null;
		}

		public override bool SuppressContextMenu
		{
			get { return true; }
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_roiGraphic != null)
				return _roiGraphic.GetCursorToken(point);

			return null;
		}

		private void OnRulerStateChanged(object sender, GraphicStateChangedEventArgs e)
		{
			
		}


		// This is temporary code.  Right now, the api is difficult to use.  
		// Ideally, we should have domain objects that make this easier.  
		private void OnRoiChanged(object sender, EventArgs e)
		{
			ROIGraphic roiGraphic = sender as ROIGraphic;
			
			PolyLineInteractiveGraphic multiLineGraphic = roiGraphic.Roi as PolyLineInteractiveGraphic;
			StandardPresentationImage image = roiGraphic.ParentPresentationImage as StandardPresentationImage;

			multiLineGraphic.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = (multiLineGraphic.AnchorPoints[1].X - multiLineGraphic.AnchorPoints[0].X);
			double heightInPixels = (multiLineGraphic.AnchorPoints[1].Y - multiLineGraphic.AnchorPoints[0].Y);
			multiLineGraphic.ResetCoordinateSystem();

			double pixelSpacingX;
			double pixelSpacingY;

			image.ImageSop.GetModalityPixelSpacing(out pixelSpacingX, out pixelSpacingY);

			bool pixelSpacingInvalid =  pixelSpacingX <= float.Epsilon ||
										pixelSpacingY <= float.Epsilon ||
										double.IsNaN(pixelSpacingX) ||
										double.IsNaN(pixelSpacingY);

			string text;

			if (pixelSpacingInvalid)
			{
				double length = Math.Sqrt(widthInPixels * widthInPixels + heightInPixels * heightInPixels);
				text = String.Format(SR.ToolsMeasurementFormatLengthPixels, length);
			}
			else
			{
				double widthInCm = widthInPixels * pixelSpacingX / 10;
				double heightInCm = heightInPixels * pixelSpacingY / 10;

				double length = Math.Sqrt(widthInCm * widthInCm + heightInCm * heightInCm);
				text = String.Format(SR.ToolsMeasurementFormatLengthCm, length);
			}

			roiGraphic.Callout.Text = text;
		}
	}
}
