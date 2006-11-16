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

	[MouseToolButton(XMouseButtons.Left, false)]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RectangularROITool : MouseTool
	{
		private static readonly string[] _disallowedModalities = { "CR", "DX", "MG" };

		public RectangularROITool()
		{
		}

		public override bool Start(MouseInformation pointerInformation)
		{
			if (pointerInformation.Tile == null || pointerInformation.Tile.PresentationImage == null)
				return true;

			base.Start(pointerInformation);

			InteractiveRectangleGraphic rectangleGraphic = new InteractiveRectangleGraphic(true);
            ROIGraphic roiGraphic = new ROIGraphic(rectangleGraphic, true);

			rectangleGraphic.StretchToken = new CursorToken(CursorToken.SystemCursors.Cross);
			rectangleGraphic.MoveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);
			roiGraphic.Callout.MoveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);
			
			roiGraphic.Callout.Text = "Area:";
			pointerInformation.Tile.PresentationImage.LayerManager.SelectedGraphicLayer.Graphics.Add(roiGraphic);
			roiGraphic.RoiChanged += new EventHandler(OnRoiChanged);

			roiGraphic.Start(pointerInformation);

			return false;
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
				text = String.Format("Area: {0:F2} pixels", area);
			}
			else
			{
				double area = Math.Abs(widthInCm * heightInCm);
				text = String.Format("Area: {0:F2} sq cm", area);
			}

			roiGraphic.Callout.Text = text;
		}
	}
}