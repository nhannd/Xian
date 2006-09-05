using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Mathematics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.DynamicOverlays;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/ToolsMeasurementRuler", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuToolsMeasurement/ToolsMeasurementRuler", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolsMeasurementRuler", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolsMeasurementRuler")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.RulerMedium.png", "Icons.RulerLarge.png")]
    
    /// <summary>
	/// Summary description for RulerTool.
	/// </summary>
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RulerTool : MouseTool
	{
		public RulerTool()
            :base(XMouseButtons.Left, false, false)
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

			if (image.ImageSop.PixelSpacingX == 0 || image.ImageSop.PixelSpacingY == 0)
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
