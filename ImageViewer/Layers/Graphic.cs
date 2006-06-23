using System;
using System.Drawing;
using System.Collections;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Layers
{
	public enum LineStyle
	{
		Solid = 0,
		Dash = 1,
		Dot = 2
	}

	/// <summary>
	/// An abstract graphical object.
	/// </summary>
	public abstract class Graphic : Layer
	{
		private Color _color = Color.Yellow;
		private LineStyle _lineStyle = LineStyle.Solid;

		protected Graphic()
		{
			this.Graphics.ItemAdded += new EventHandler<LayerEventArgs>(OnGraphicAdded);
		}

		protected Graphic(bool isPrimitive) : base(isPrimitive)
		{
		}

		/// <summary>
		/// Gets a collection of <see cref="Graphic"/> objects.
		/// </summary>
		public GraphicCollection Graphics
		{
			get
			{
				return base.BaseLayers as GraphicCollection;
			}
		}

		public override bool Selected
		{
			get { return base.Selected; }	// this is required for compilation on MONO
			set
			{
				Platform.CheckMemberIsSet(base.ParentLayerManager, "Layer.ParentLayerManager");

				if (base.Selected != value)
				{
					base.Selected = value;

					if (base.Selected)
					{
						if (base.ParentLayerManager != null)
							base.ParentLayerManager.SelectedGraphic = this;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the colour of the <see cref="Graphic"/>.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;

				if (this.IsLeaf)
					return;

				foreach (Graphic graphic in this.Graphics)
					graphic.Color = value;
			}
		}

		/// <summary>
		/// Gets or sets the line style used for the <see cref="Graphic"/>.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return _lineStyle; }
			set
			{
				_lineStyle = value;

				if (this.IsLeaf)
					return;

				foreach (Graphic graphic in this.Graphics)
					graphic.LineStyle = value;
			}
		}

		/// <summary>
		/// Returns a value indicating whether the mouse is over the <see cref="Graphic"/>.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public abstract bool HitTest(XMouseEventArgs e);

		public override void Draw()
		{
			Platform.CheckMemberIsSet(this.ParentPresentationImage, "PresentationImage");
			this.RedrawRequired = true;
			this.ParentPresentationImage.DrawLayers(true);
		}

		/// <summary>
		/// Moves the <see cref="Graphic"/> by a specified distance.
		/// </summary>
		/// <param name="delta"></param>
		/// <remarks>Depending on the value of <see cref="CoordinateSystem"/>,
		/// <i>delta</i> will be interpreted in either source
		/// or destination coordinates.</remarks>
		public virtual void Move(SizeF delta)
		{
			if (this.IsLeaf)
				return;

			foreach (Graphic graphic in this.Graphics)
				graphic.Move(delta);
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

		protected override BaseLayerCollection CreateChildLayers()
		{
			return new GraphicCollection();
		}

		private void OnGraphicAdded(object sender, LayerEventArgs e)
		{
			e.Layer.ParentLayer = this;
			e.Layer.ParentLayerManager = base.ParentLayerManager;
			e.Layer.SpatialTransform = base.SpatialTransform;
			e.Layer.CoordinateSystem = base.CoordinateSystem;
		}
	}
}
