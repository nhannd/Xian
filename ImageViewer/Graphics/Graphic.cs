using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	public enum CoordinateSystem
	{
		Source = 0,
		Destination = 1
	}

	/// <summary>
	/// The base layer object.
	/// </summary>
	public abstract class Graphic : IGraphic
	{
		private IGraphic _parentGraphic;
		private IImageViewer _parentImageViewer;
		private IPresentationImage _parentPresentationImage;

		private string _name;
		private bool _visible = true;
		private SpatialTransform _spatialTransform;
		private Stack<CoordinateSystem> _coordinateSystemStack = new Stack<CoordinateSystem>();

		protected Graphic()
		{
			_coordinateSystemStack.Push(CoordinateSystem.Source);
		}

		/// <summary>
		/// Gets this layer's parent <see cref="Layer"/>.
		/// </summary>
		public IGraphic ParentGraphic
		{
			get { return _parentGraphic; }
		}

		internal void SetParentGraphic(IGraphic parentGraphic)
		{
			_parentGraphic = parentGraphic;
		}

		/// <summary>
		/// Gets this layer's parent <see cref="PresentationImage"/>.
		/// </summary>
		/// <value>Can be <b>null</b> if the layer has not been added
		/// to the layer tree  (For example, right after construction.)</value>
		public virtual IPresentationImage ParentPresentationImage
		{
			get { return _parentPresentationImage; }
		}

		internal virtual void SetParentPresentationImage(IPresentationImage parentPresentationImage)
		{
			_parentPresentationImage = parentPresentationImage;
		}

		/// <summary>
		/// Gets this layer's parent <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>Can be <b>null</b> if the layer has not been added
		/// to the layer tree  (For example, right after construction.)</value>
		public virtual IImageViewer ImageViewer
		{
			get { return _parentImageViewer; }
		}

		internal virtual void SetImageViewer(IImageViewer imageViewer)
		{
			_parentImageViewer = imageViewer;
		}

		/// <summary>
		/// Gets or sets the name of this layer.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this layer is visible.
		/// </summary>
		/// <remarks>The effect of this property will be seen when 
		/// <see cref="Draw"/> is called. This property is recursive.  That is, 
		/// when set, the new value is applied to all child layers, right 
		/// down to the leaves.
		/// </remarks>
		public virtual bool Visible
		{
			get { return _visible; }
			set	{ _visible = value; }
		}

		/// <summary>
		/// Gets or sets the coordinate system the layer is using.
		/// </summary>
		/// <remarks>
		/// It is often desirable for a client to be able to work in both original
		/// image (source) coordinates, or in screen (destination) coordinates.
		/// By setting this property, all coordinates will be interpreted in that
		/// coordinate system for this layer and all its child layers as well.
		/// The proper practice is to call <see cref="ResetCoordinateSystem"/> after
		/// having set this property.
		/// This property is recursive.  That is, when set, the
		/// new value is applied to all child layers, right down to the leaves.		
		/// </remarks>
		public virtual CoordinateSystem CoordinateSystem
		{
			get { return _coordinateSystemStack.Peek(); }
			set
			{
				Platform.CheckForNullReference(value, "CoordinateSystem");
				_coordinateSystemStack.Push(value);
			}
		}

		/// <summary>
		/// Gets the <see cref="SpatialTransform"/> associated with this
		/// layer's parent <see cref="LayerGroup"/>
		/// </summary>
		public virtual SpatialTransform SpatialTransform
		{
			get 
			{
				if (_spatialTransform == null)
					_spatialTransform = CreateSpatialTransform();

				return _spatialTransform;
			}
			internal set { _spatialTransform = value; }
		}

		/// <summary>
		/// Returns a value indicating whether the mouse is over the <see cref="Graphic"/>.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public abstract bool HitTest(Point point);

		/// <summary>
		/// Moves the <see cref="Graphic"/> by a specified distance.
		/// </summary>
		/// <param name="delta"></param>
		/// <remarks>Depending on the value of <see cref="CoordinateSystem"/>,
		/// <i>delta</i> will be interpreted in either source
		/// or destination coordinates.</remarks>
		public abstract void Move(SizeF delta);

		/// <summary>
		/// Resets the <see cref="CoordinateSystem"/> to its old value.
		/// </summary>
		public virtual void ResetCoordinateSystem()
		{
			if (_coordinateSystemStack.Count == 1)
				return;

			_coordinateSystemStack.Pop();
		}

		/// <summary>
		/// Draw the layer.
		/// </summary>
		public virtual void Draw()
		{
			Platform.CheckMemberIsSet(this.ParentPresentationImage, "PresentationImage");
			this.ParentPresentationImage.Draw();
		}

		protected virtual SpatialTransform CreateSpatialTransform()
		{
			return new SpatialTransform(this);
		}

	}
}
