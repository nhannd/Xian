using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics.Tests
{
	[MenuAction("activate", "global-menus/Debug/CreateShapesPlaypen", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/Debug/CreateShapesPlaypen", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("cxt", "shopes/Test Cxt Action", "Dummy")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal class DrawEllipTestTool : ImageViewerTool
	{
		public DrawEllipTestTool() {}

		public void Dummy() {}

		private IGraphic Decorate(IGraphic graphic)
		{
			return new ContextMenuControlGraphic("shopes", this.Actions, new MoveControlGraphic(graphic));
		}

		private IGraphic Decorate(ILineSegmentGraphic graphic)
		{
			return new LineSegmentStretchControlGraphic(new MoveControlGraphic(graphic));
		}

		private IGraphic Decorate(ITextGraphic graphic)
		{
			return new ContextMenuControlGraphic("shopes", this.Actions, new TextEditControlGraphic(new MoveControlGraphic(graphic)));
		}

		private IGraphic Decorate(IBoundableGraphic graphic)
		{
			return new ContextMenuControlGraphic("shopes", this.Actions, new BoundableStretchControlGraphic(new BoundableResizeControlGraphic(1f, new MoveControlGraphic(graphic))));
		}

		private IEnumerable<IGraphic> CreateGraphics()
		{
			EllipsePrimitive ellip = new EllipsePrimitive();
			ellip.TopLeft = new PointF(50, 50);
			ellip.BottomRight = new PointF(100, 100);
			ellip.Color = Color.LimeGreen;
			yield return Decorate(ellip);

			RectanglePrimitive rect = new RectanglePrimitive();
			rect.TopLeft = new PointF(50, 50);
			rect.BottomRight = new PointF(100, 100);
			rect.Color = Color.CadetBlue;
			yield return Decorate(rect);

			LinePrimitive line = new LinePrimitive();
			line.Pt1 = new PointF(25, 75);
			line.Pt2 = new PointF(50, 100);
			line.Color = Color.Lime;
			yield return Decorate(line);

			InvariantTextPrimitive text = new InvariantTextPrimitive();
			text.AnchorPoint = new PointF(150, 150);
			text.Text = "It's turtles all the way down";
			text.Color = Color.LightGoldenrodYellow;
			yield return Decorate(text);

			CompositeGraphic compo = new CompositeGraphic();
			compo.Graphics.Add(ellip.Clone());
			compo.Graphics.Add(rect.Clone());
			foreach (IVectorGraphic g in compo.Graphics)
				g.Color = Color.SkyBlue;
			yield return Decorate(compo);
		}

		public void Select()
		{
			foreach (IGraphic graphic in this.CreateGraphics())
			{
				IStandardStatefulGraphic statefulGraphic = new RoiGraphic(graphic);
				statefulGraphic.State = statefulGraphic.CreateInactiveState();
				base.SelectedOverlayGraphicsProvider.OverlayGraphics.Add(statefulGraphic); //
			}
			//base.SelectedOverlayGraphicsProvider.OverlayGraphics.Add(new XCalloutGraphic("asdfsdfdsfdsfsdfdsfsdfsd"));
			base.SelectedPresentationImage.Draw();
		}
	}
}