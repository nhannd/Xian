using System;
using System.Drawing;
using System.Threading;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal static class InteractiveShutterGraphicBuilders
	{
		private static readonly Color _normalColor = Color.LightSteelBlue;
		private static readonly Color _invalidColor = Color.Red;
		private static readonly int _flashDelay = 100;

		public static InteractiveCircleGraphicBuilder CreateCircularShutterBuilder(IBoundableGraphic graphic)
		{
			return new InteractiveCircularShutterBuilder(graphic);
		}

		public static InteractiveBoundableGraphicBuilder CreateRectangularShutterBuilder(IBoundableGraphic graphic)
		{
			return new InteractiveRectangularShutterBuilder(graphic);
		}

		public static InteractivePolygonGraphicBuilder CreatePolygonalShutterBuilder(IPointsGraphic graphic)
		{
			return new InteractivePolygonalShutterBuilder(graphic);
		}

		private static bool IsShutterTooSmall(IGraphic shutterGraphic)
		{
			shutterGraphic.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				RectangleF boundingBox = shutterGraphic.BoundingBox;
				return (boundingBox.Width < 50 || boundingBox.Height < 50);
			}
			finally
			{
				shutterGraphic.ResetCoordinateSystem();
			}
		}

		private class FlashGraphicColorController
		{
			private delegate void DoFlashDelegate(SynchronizationContext context);

			private readonly IVectorGraphic _graphic;

			private FlashGraphicColorController(IVectorGraphic graphic)
			{
				_graphic = graphic;
			}

			private void DoFlash(SynchronizationContext context)
			{
				context.Send(delegate
				             	{
									_graphic.Color = _invalidColor;
				             		_graphic.Draw();
				             	}, null);
				Thread.Sleep(_flashDelay);
				context.Send(delegate
				             	{
				             		_graphic.Color = _normalColor;
				             		_graphic.Draw();
				             	}, null);
			}

			public static void Flash(IGraphic graphic)
			{
				if (graphic is IVectorGraphic)
				{
					FlashGraphicColorController controller = new FlashGraphicColorController((IVectorGraphic) graphic);
					DoFlashDelegate doFlashDelegate = controller.DoFlash;
					doFlashDelegate.BeginInvoke(SynchronizationContext.Current, delegate(IAsyncResult result) { doFlashDelegate.EndInvoke(result); }, null);
				}
			}
		}

		private class InteractiveCircularShutterBuilder : InteractiveCircleGraphicBuilder
		{
			public InteractiveCircularShutterBuilder(IBoundableGraphic graphic)
				: base(graphic)
			{
				graphic.Color = _normalColor;
			}

			protected override void NotifyGraphicComplete()
			{
				if (IsShutterTooSmall(base.Graphic))
				{
					FlashGraphicColorController.Flash(base.Graphic);
					base.Rollback();
					return;
				}

				base.NotifyGraphicComplete();
			}
		}

		private class InteractiveRectangularShutterBuilder : InteractiveBoundableGraphicBuilder
		{
			public InteractiveRectangularShutterBuilder(IBoundableGraphic graphic)
				: base(graphic)
			{
				graphic.Color = _normalColor;
			}

			protected override void NotifyGraphicComplete()
			{
				if (IsShutterTooSmall(base.Graphic))
				{
					FlashGraphicColorController.Flash(base.Graphic);
					base.Rollback();
					return;
				}

				base.NotifyGraphicComplete();
			}
		}

		private class InteractivePolygonalShutterBuilder : InteractivePolygonGraphicBuilder
		{
			public InteractivePolygonalShutterBuilder(IPointsGraphic graphic)
				: base(graphic)
			{
				graphic.Color = _normalColor;
			}

			protected override void NotifyGraphicComplete()
			{
				if (IsShutterTooSmall(base.Graphic))
				{
					FlashGraphicColorController.Flash(base.Graphic);
					base.Rollback();
					return;
				}

				base.NotifyGraphicComplete();
			}
		}
	}
}