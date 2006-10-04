using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.DynamicOverlays;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/ToolsMeasurementRuler", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuToolsMeasurement/ToolsMeasurementRuler", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolsMeasurementRuler", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolsMeasurementRuler")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.RulerMedium.png", "Icons.RulerLarge.png")]
    
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RulerTool : MouseTool
	{
		private static readonly string[] _disallowedModalities = { "CR", "DX", "MG" };

		public RulerTool()
            :base(XMouseButtons.Left, false)
		{
		}

		#region IUIEventHandler Members

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.SelectedTile == null ||
				e.SelectedPresentationImage == null)
				return true;

			// Create a new ruler; subsequent UI messages will be handled by the
			// ruler itself.
			InteractiveMultiLineGraphic multilineGraphic = new InteractiveMultiLineGraphic(true, 2);
			ROIGraphic roiGraphic = new ROIGraphic(multilineGraphic, true);
			roiGraphic.Callout.Text = "Line ROI";
			e.SelectedPresentationImage.LayerManager.SelectedGraphicLayer.Graphics.Add(roiGraphic);
			roiGraphic.RoiChanged += new EventHandler(OnRoiChanged);
			roiGraphic.OnMouseDown(e);

			return false;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			base.OnMouseMove(e);

			return false;
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			return false;
		}

		public override bool OnMouseWheel(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return true;
		}

		public override bool OnKeyDown(XKeyEventArgs e)
		{
			return false;
		}

		public override bool OnKeyUp(XKeyEventArgs e)
		{
			return false;
		}

		#endregion

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
			double widthInCm = widthInPixels * image.ImageSop.PixelSpacingX / 10;
			double heightInCm = heightInPixels * image.ImageSop.PixelSpacingY / 10;
			multiLineGraphic.ResetCoordinateSystem();

			string text;

			bool pixelSpacingInvalid = image.ImageSop.PixelSpacingX <= float.Epsilon ||
										image.ImageSop.PixelSpacingY <= float.Epsilon ||
										double.IsNaN(image.ImageSop.PixelSpacingX) ||
										double.IsNaN(image.ImageSop.PixelSpacingY);

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
