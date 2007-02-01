using System;
using System.Drawing;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Graphics
{
	[ExtensionPoint()]
	public class GraphicToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IGraphicToolContext : IToolContext
	{
		//you can go all the way up the chain to the imageviewer, so this is the only property needed.
		IGraphic Graphic { get; }
	}

	public class GraphicToolContext : ToolContext, IGraphicToolContext
	{
		private IGraphic _graphic;

		public GraphicToolContext(IGraphic graphic)
		{
			_graphic = graphic;
		}

		public IGraphic Graphic
		{
			get { return _graphic; }
		}
	}

	public enum LineStyle
	{
		Solid = 0,
		Dash = 1,
		Dot = 2
	}

	/// <summary>
	/// An abstract graphical object.
	/// </summary>
	public abstract class VectorGraphic : Graphic
	{
		private Color _color = Color.Yellow;
		private LineStyle _lineStyle = LineStyle.Solid;

		protected VectorGraphic()
		{
		}

		/// <summary>
		/// Gets or sets the colour of the <see cref="Graphic"/>.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}

		/// <summary>
		/// Gets or sets the line style used for the <see cref="Graphic"/>.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return _lineStyle; }
			set { _lineStyle = value; }
		}

		/// <summary>
		/// Returns a <see cref="SizeF"/> representing the distance between two points.
		/// </summary>
		/// <param name="lastPoint"></param>
		/// <param name="currentPoint"></param>
		/// <returns></returns>
		public static SizeF CalcGraphicPositionDelta(PointF lastPoint, PointF currentPoint)
		{
			float deltaX = currentPoint.X - lastPoint.X;
			float deltaY = currentPoint.Y - lastPoint.Y;

			return new SizeF(deltaX, deltaY);
		}
	}
}
