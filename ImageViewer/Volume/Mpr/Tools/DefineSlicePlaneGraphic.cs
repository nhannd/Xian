using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	partial class DefineSlicePlaneTool
	{
		private class SliceLineGraphic : CompositeGraphic
		{
			private SliceLineGraphic(IGraphic graphic)
			{
				base.Graphics.Add(graphic);
			}

			public IPointsGraphic LineGraphic
			{
				get { return ((IControlGraphic) base.Graphics[0]).Subject as IPointsGraphic; }
			}

			private SliceLineControlGraphic GetSliceLineControlGraphic(IControlGraphic graphic)
			{
				if (graphic is SliceLineControlGraphic)
					return (SliceLineControlGraphic) graphic;

				if (graphic.DecoratedGraphic is IControlGraphic)
					return GetSliceLineControlGraphic((IControlGraphic) graphic.DecoratedGraphic);

				return null;
			}

			public string Text
			{
				get { return GetSliceLineControlGraphic((IControlGraphic) base.Graphics[0]).Text; }
				set { GetSliceLineControlGraphic((IControlGraphic) base.Graphics[0]).Text = value; }
			}

			/// <summary>
			/// Moves the graphic from where ever it is to the target image.
			/// </summary>
			public static void TranslocateGraphic(SliceLineGraphic graphic, IPresentationImage targetImage)
			{
				IPresentationImage oldImage = graphic.ParentPresentationImage;
				if (oldImage != targetImage)
				{
					RemoveSliceLineGraphic(graphic);
					if (oldImage != null)
						oldImage.Draw();
					AddSliceLineGraphic(graphic, targetImage);
				}
			}

			public static SliceLineGraphic CreateSliceLineGraphic(MprViewerComponent mprViewer, IMprSliceSet sliceSet, Color hotColor, Color normalColor)
			{
				PolylineGraphic polylineGraphic = new PolylineGraphic();
				// TODO JY - figure out what to initialize the ends points to based on sliceSet
				MoveControlGraphic moveControlGraphic = new MoveControlGraphic(polylineGraphic);
				VerticesControlGraphic verticesControlGraphic = new VerticesControlGraphic(moveControlGraphic);
				verticesControlGraphic.CanAddRemoveVertices = false;
				SliceLineControlGraphic sliceLineControlGraphic = new SliceLineControlGraphic(verticesControlGraphic);
				StandardStatefulGraphic statefulGraphic = new StandardStatefulGraphic(sliceLineControlGraphic);
				statefulGraphic.InactiveColor = statefulGraphic.SelectedColor = normalColor;
				statefulGraphic.FocusColor = statefulGraphic.FocusSelectedColor = hotColor;
				statefulGraphic.State = statefulGraphic.CreateInactiveState();
				return new SliceLineGraphic(statefulGraphic);
			}

			public static void AddSliceLineGraphic(SliceLineGraphic graphic, IPresentationImage image)
			{
				IApplicationGraphicsProvider applicationGraphicsProvider = image as IApplicationGraphicsProvider;
				if (applicationGraphicsProvider != null)
				{
					applicationGraphicsProvider.ApplicationGraphics.Add(graphic);
				}
			}

			public static void RemoveSliceLineGraphic(SliceLineGraphic graphic)
			{
				IApplicationGraphicsProvider applicationGraphicsProvider = graphic.ParentPresentationImage as IApplicationGraphicsProvider;
				if (applicationGraphicsProvider != null)
				{
					applicationGraphicsProvider.ApplicationGraphics.Remove(graphic);
				}
			}

			private class SliceLineControlGraphic : ControlGraphic
			{
				private ITextGraphic _textGraphic;

				public SliceLineControlGraphic(IGraphic subject) : base(subject)
				{
					base.Graphics.Add(_textGraphic = new InvariantTextPrimitive());
					base.DecoratedGraphic.VisualStateChanged += DecoratedGraphic_VisualStateChanged;
				}

				protected override void Dispose(bool disposing)
				{
					base.DecoratedGraphic.VisualStateChanged -= DecoratedGraphic_VisualStateChanged;
					_textGraphic = null;

					base.Dispose(disposing);
				}

				public string Text
				{
					get { return _textGraphic.Text; }
					set { _textGraphic.Text = value; }
				}

				private void DecoratedGraphic_VisualStateChanged(object sender, VisualStateChangedEventArgs e)
				{
					base.DecoratedGraphic.CoordinateSystem = CoordinateSystem.Destination;
					_textGraphic.CoordinateSystem = CoordinateSystem.Destination;
					try
					{
						RectangleF rectangle = base.DecoratedGraphic.BoundingBox;
						_textGraphic.Location = new PointF(rectangle.Right, rectangle.Top);
					}
					finally
					{
						_textGraphic.ResetCoordinateSystem();
						base.DecoratedGraphic.ResetCoordinateSystem();
					}
				}
			}
		}
	}
}