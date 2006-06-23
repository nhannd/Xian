using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;
using ClearCanvas.Workstation.Model.Imaging;
using ClearCanvas.Workstation.Model.DynamicOverlays;
using ClearCanvas.Common.Application.Tools;
using ClearCanvas.Common.Application.Actions;

namespace ClearCanvas.Workstation.Tools.Measurement
{
    [MenuAction("activate", "MenuTools/MenuToolsMeasurement/ToolsMeasurementRectangularROI", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "ToolbarMeasurement/ToolsMeasurementRectangularROI", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolsMeasurementRectangularROI")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.RectangularROIMedium.png", "Icons.RectangularROILarge.png")]
    
    /// <summary>
	/// Summary description for RulerTool.
	/// </summary>
    [ExtensionOf(typeof(ClearCanvas.Workstation.Model.ImageWorkspaceToolExtensionPoint))]
    public class RectangularROITool : MouseTool
	{
		public RectangularROITool()
            :base(XMouseButtons.Right, false)
		{
		}

		#region IUIEventHandler Members

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.SelectedTile == null ||
				e.SelectedPresentationImage == null)
				return true;

			InteractiveRectangleGraphic rectangleGraphic = new InteractiveRectangleGraphic(true);
            ROIGraphic roiGraphic = new ROIGraphic(rectangleGraphic, true);
			roiGraphic.Callout.Text = "Area:";
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

		private void OnRoiChanged(object sender, EventArgs e)
		{
			ROIGraphic roiGraphic = sender as ROIGraphic;
			InteractiveRectangleGraphic rectangleGraphic = roiGraphic.Roi as InteractiveRectangleGraphic;
			DicomPresentationImage image = roiGraphic.ParentPresentationImage as DicomPresentationImage;

			rectangleGraphic.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = (rectangleGraphic.BottomRight.X - rectangleGraphic.TopLeft.X);
			double heightInPixels = (rectangleGraphic.BottomRight.Y - rectangleGraphic.TopLeft.Y);
			double widthInCm = widthInPixels * image.ImageSop.PixelSpacingX / 10;
			double heightInCm = heightInPixels * image.ImageSop.PixelSpacingY / 10;
			rectangleGraphic.ResetCoordinateSystem();

			string text;

			if (image.ImageSop.PixelSpacingX == 0 || image.ImageSop.PixelSpacingY == 0)
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