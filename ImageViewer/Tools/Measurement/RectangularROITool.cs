using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.DynamicOverlays;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
    [MenuAction("activate", "global-menus/MenuTools/MenuToolsMeasurement/ToolsMeasurementRectangularROI", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolsMeasurementRectangularROI", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolsMeasurementRectangularROI")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.RectangularROIMedium.png", "Icons.RectangularROILarge.png")]
	[GroupHint("activate", "Tools.Image.Measurement.ROI.Rectangular")]

	[MouseToolButton(XMouseButtons.Left, false)]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RectangularROITool : MouseTool
	{
		private static readonly string[] _disallowedModalities = { "CR", "DX", "MG" };
		private ROIGraphic _createGraphic;

		public RectangularROITool()
		{
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (!IsImageValid(mouseInformation.Tile.PresentationImage)) 
				return false;

			if (_createGraphic != null)
				return _createGraphic.Start(mouseInformation);

			//When you create a graphic from within a tool (particularly one that needs capture, like a multi-click graphic),
			//see it through to the end of creation.  It's just cleaner, not to mention that if this tool knows how to create it,
			//it should also know how to (and be responsible for) cancelling it and/or deleting it appropriately.
			InteractiveRectangleGraphic rectangleGraphic = new InteractiveRectangleGraphic(true);
			_createGraphic = new ROIGraphic(rectangleGraphic, true);

			rectangleGraphic.StretchToken = new CursorToken(CursorToken.SystemCursors.Cross);
			rectangleGraphic.MoveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);
			_createGraphic.Callout.MoveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);

			_createGraphic.Callout.Text = SR.ToolsMeasurementArea;
			mouseInformation.Tile.PresentationImage.LayerManager.SelectedGraphicLayer.Graphics.Add(_createGraphic);
			_createGraphic.RoiChanged += new EventHandler(OnRoiChanged);

			if (_createGraphic.Start(mouseInformation))
				return true;

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_createGraphic != null)
				return _createGraphic.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_createGraphic != null)
			{
				if (_createGraphic.Stop(mouseInformation))
					return true;
			}

			_createGraphic = null;
			return false;
		}

		public override void Cancel()
		{
			if (_createGraphic != null)
				_createGraphic.Cancel();

			_createGraphic.ParentLayerManager.SelectedGraphicLayer.Graphics.Remove(_createGraphic);
			_createGraphic = null;
		}

		public override bool SuppressContextMenu
		{
			get { return true; }
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_createGraphic != null)
				return _createGraphic.GetCursorToken(point);

			return null;
		}

		private bool PixelSpacingNotAllowed(ImageSop imageSop)
		{
			foreach (string modality in _disallowedModalities)
			{
				if (String.Compare(modality, imageSop.Modality, true) == 0)
					return true;
			}

			return false;
		}

		private void OnRoiChanged(object sender, EventArgs e)
		{
			ROIGraphic roiGraphic = sender as ROIGraphic;

			InteractiveRectangleGraphic rectangleGraphic = roiGraphic.Roi as InteractiveRectangleGraphic;
			DicomPresentationImage image = roiGraphic.ParentPresentationImage as DicomPresentationImage;

			rectangleGraphic.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = (rectangleGraphic.BottomRight.X - rectangleGraphic.TopLeft.X);
			double heightInPixels = (rectangleGraphic.BottomRight.Y - rectangleGraphic.TopLeft.Y);
			double widthInCm = widthInPixels * image.ImageSop.PixelSpacing.Column / 10;
			double heightInCm = heightInPixels * image.ImageSop.PixelSpacing.Row / 10;
			rectangleGraphic.ResetCoordinateSystem();

			string text;

			bool pixelSpacingInvalid = image.ImageSop.PixelSpacing.Column <= float.Epsilon ||
										image.ImageSop.PixelSpacing.Row <= float.Epsilon ||
										double.IsNaN(image.ImageSop.PixelSpacing.Column) ||
										double.IsNaN(image.ImageSop.PixelSpacing.Row);

			//!! This has been put in as a temporary measure to stop certain modality 
			//!! images (DX, CR, MG) from reporting the incorrect measurements in cm.
			//!! These modalities should actually use Imager Pixel Spacing for the calculation.

			if (this.PixelSpacingNotAllowed(image.ImageSop))
				pixelSpacingInvalid = true;

			if (pixelSpacingInvalid)
			{
				double area = Math.Abs(widthInPixels * heightInPixels);
				text = String.Format(SR.ToolsMeasurementFormatAreaPixels, area);
			}
			else
			{
				double area = Math.Abs(widthInCm * heightInCm);
				text = String.Format(SR.ToolsMeasurementFormatAreaSquareCm, area);
			}

			roiGraphic.Callout.Text = text;
		}
	}
}