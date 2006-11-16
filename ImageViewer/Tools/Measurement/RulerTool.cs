using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.DynamicOverlays;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/ToolsMeasurementRuler", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuToolsMeasurement/ToolsMeasurementRuler", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolsMeasurementRuler", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolsMeasurementRuler")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.RulerMedium.png", "Icons.RulerLarge.png")]

	[MouseToolButton(XMouseButtons.Right, false)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RulerTool : MouseTool
	{
		private static readonly string[] _disallowedModalities = { "CR", "DX", "MG" };

		public RulerTool()
		{
		}

		public override bool Start(MouseInformation pointerInformation)
		{
			base.Start(pointerInformation);

			if (pointerInformation.Tile == null || pointerInformation.Tile.PresentationImage == null)
				return true;

			// Create a new ruler; subsequent UI messages will be handled by the
			// ruler itself.
			InteractiveMultiLineGraphic multilineGraphic = new InteractiveMultiLineGraphic(true, 2);
			ROIGraphic roiGraphic = new ROIGraphic(multilineGraphic, true);

			multilineGraphic.StretchToken = new CursorToken(CursorToken.SystemCursors.Cross);
			multilineGraphic.MoveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);
			roiGraphic.Callout.MoveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);

			roiGraphic.Callout.Text = "Line ROI";
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

		// This is temporary code.  Right now, the api is difficult to use.  
		// Ideally, we should have domain objects that make this easier.  
		private void OnRoiChanged(object sender, EventArgs e)
		{
			ROIGraphic roiGraphic = sender as ROIGraphic;
			InteractiveMultiLineGraphic multiLineGraphic = roiGraphic.Roi as InteractiveMultiLineGraphic;
			DicomPresentationImage image = roiGraphic.ParentPresentationImage as DicomPresentationImage;

			multiLineGraphic.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = (multiLineGraphic.AnchorPoints[1].X - multiLineGraphic.AnchorPoints[0].X);
			double heightInPixels = (multiLineGraphic.AnchorPoints[1].Y - multiLineGraphic.AnchorPoints[0].Y);
			double widthInCm = widthInPixels * image.ImageSop.PixelSpacing.Column / 10;
			double heightInCm = heightInPixels * image.ImageSop.PixelSpacing.Row / 10;
			multiLineGraphic.ResetCoordinateSystem();

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
				double length = Math.Sqrt(widthInPixels * widthInPixels + heightInPixels * heightInPixels);
				text = String.Format("Length: {0:F2} pixels", length);
			}
			else
			{
				double length = Math.Sqrt(widthInCm * widthInCm + heightInCm * heightInCm);
				text = String.Format("Length: {0:F2} cm", length);
			}

			roiGraphic.Callout.Text = text;
		}
	}
}
