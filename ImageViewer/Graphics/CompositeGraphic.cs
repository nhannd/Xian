using System;
using System.Drawing;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A group of graphics that are subject to a particular <see cref="SpatialTransform"/>.
	/// </summary>
	public class CompositeGraphic : Graphic
	{
		private GraphicCollection _graphics;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeGraphic"/> class.
		/// </summary>
		public CompositeGraphic()
		{
			this.Graphics.ItemAdded += new EventHandler<GraphicEventArgs>(OnGraphicAdded);
		}

		/// <summary>
		/// Gets a collection of this <see cref="CompositeGraphic"/>'s child layers.
		/// </summary>
		public GraphicCollection Graphics
		{
			get 
			{
				if (_graphics == null)
					_graphics = new GraphicCollection();

				return _graphics;
			}
		}

		public override bool Visible
		{
			get { return base.Visible; }
			set
			{
				base.Visible = value;

				foreach (Graphic graphic in this.Graphics)
					graphic.Visible = value;
			}
		}
		
		public override CoordinateSystem CoordinateSystem
		{
			get { return base.CoordinateSystem; }
			set
			{
				base.CoordinateSystem = value;

				foreach (Graphic graphic in this.Graphics)
					graphic.CoordinateSystem = value;
			}
		}

		public override void ResetCoordinateSystem()
		{
			base.ResetCoordinateSystem();

			foreach (Graphic graphic in this.Graphics)
				graphic.ResetCoordinateSystem();
		}

		internal override void SetImageViewer(IImageViewer imageViewer)
		{
			base.SetImageViewer(imageViewer);

			foreach (Graphic graphic in this.Graphics)
				graphic.SetImageViewer(imageViewer);
		}

		internal override void SetParentPresentationImage(IPresentationImage parentPresentationImage)
		{
			base.SetParentPresentationImage(parentPresentationImage);

			foreach (Graphic graphic in this.Graphics)
				graphic.SetParentPresentationImage(parentPresentationImage);
		}

		public override bool HitTest(Point point)
		{
			foreach (Graphic graphic in this.Graphics)
			{
				if (graphic.HitTest(point))
					return true;
			}

			return false;
		}

		public override void Move(SizeF delta)
		{
			foreach (Graphic graphic in this.Graphics)
				graphic.Move(delta);
		}

		private void OnGraphicAdded(object sender, GraphicEventArgs e)
		{
			Graphic graphic = e.Graphic as Graphic;

			graphic.SetParentGraphic(this);
			graphic.SetParentPresentationImage(this.ParentPresentationImage);
			graphic.SetImageViewer(this.ImageViewer);
			graphic.Visible = this.Visible;
			graphic.CoordinateSystem = this.CoordinateSystem;
		}
	}
}
