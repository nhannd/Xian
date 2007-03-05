using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Coordinate system enumeration.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Consider a 512x512 image that has anchored to it a line
	/// defined by points P1(0,0) and P2(10,10), where those points are
	/// in the coordinate system of the image.  In that case, the line is said
	/// to be in <i>source</i> coordinates.  Now consider if the image 
	/// is zoomed by 2x where the center of expansion is (0,0).
	/// In that case, the line's source coordinates are still the same, but 
	/// its <i>destination</i> or Tile coordinates are now P1'(0,0), P2'(20,20).
	/// </para>
	/// <para>
	/// In general, the source coordinates of a particular <see cref="Graphic"/>
	/// are coordinates in the coordinate system of its immediate parent.
	/// Destination coordinates are coordinates in the coordinate system
	/// of the root of the scene graph, i.e., the Tile.  Put another way,
	/// destination coordinates = T(source coordinates), where T represents
	/// the a graphic's cumulative transformation.
	/// </para>
	/// </remarks>
	public enum CoordinateSystem
	{
		/// <summary>
		/// Represent a <see cref="Graphic"/> object's coordinates in
		/// its immediate parent's, or <i>source</i> coordinate system.
		/// </summary>
		Source = 0,

		/// <summary>
		/// Represent a <see cref="Graphic"/> object's coordinates in
		/// the coordinate system of the root of the scene graph, that is,
		/// in Tile or <i>destination</i> coordinates.
		/// </summary>
		Destination = 1
	}

	/// <summary>
	/// An graphical object that can be rendered.
	/// </summary>
	public abstract class Graphic : IGraphic
	{
		#region Private fields

		private IGraphic _parentGraphic;
		private IImageViewer _parentImageViewer;
		private IPresentationImage _parentPresentationImage;

		private string _name;
		private bool _visible = true;
		private SpatialTransform _spatialTransform;
		private Stack<CoordinateSystem> _coordinateSystemStack = new Stack<CoordinateSystem>();

		#endregion

		/// <summary>
		/// 
		/// </summary>
		protected Graphic()
		{
			_coordinateSystemStack.Push(CoordinateSystem.Source);
		}

		/// <summary>
		/// Gets this <see cref="Graphic"/> object's parent <see cref="Graphic"/>.
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
		/// Gets this <see cref="Graphic"/> object's associated 
		/// <see cref="IPresentationImage"/>.
		/// </summary>
		/// <value>The associated <see cref="IPresentationImage"/> or <b>null</b>
		/// if the <see cref="Graphic"/> is not yet part of the scene graph.</value>
		public virtual IPresentationImage ParentPresentationImage
		{
			get { return _parentPresentationImage; }
		}

		internal virtual void SetParentPresentationImage(IPresentationImage parentPresentationImage)
		{
			_parentPresentationImage = parentPresentationImage;
		}

		/// <summary>
		/// Gets this <see cref="Graphic"/> object's associated 
		/// <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b>
		/// if the <see cref="Graphic"/> is not yet associated with
		/// an <see cref="IImageViewer"/>.</value>
		public virtual IImageViewer ImageViewer
		{
			get { return _parentImageViewer; }
		}

		internal virtual void SetImageViewer(IImageViewer imageViewer)
		{
			_parentImageViewer = imageViewer;
		}

		/// <summary>
		/// Gets or sets the name of this <see cref="Graphic"/>.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Graphic"/> is visible.
		/// </summary>
		public virtual bool Visible
		{
			get { return _visible; }
			set	{ _visible = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// After the <see cref="Graphic.CoordinateSystem"/> has been set and the
		/// desired operations have been performed on the <see cref="Graphic"/>,
		/// it is proper practice to call <see cref="ResetCoordinateSystem"/>
		/// to restore the previous coordinate system.
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
		/// Gets this <see cref="Graphic"/> object's <see cref="SpatialTransform"/>.
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
		/// Performs a hit test on the <see cref="Graphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="Graphic"/>,
		/// <b>false</b> otherwise.
		/// <remarks>
		/// It is up to the <see cref="Graphic"/> to define what a "hit" is.
		/// </remarks>
		/// </returns>
		public abstract bool HitTest(Point point);

		/// <summary>
		/// Moves the <see cref="Graphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public abstract void Move(SizeF delta);

		/// <summary>
		/// Resets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="ResetCoordinateSystem"/> will reset the <see cref="CoordinateSystem"/>
		/// to what it was before the <see cref="Graphic.CoordinateSystem"/> property
		/// was last set.
		/// </para>
		/// </remarks>
		public virtual void ResetCoordinateSystem()
		{
			if (_coordinateSystemStack.Count == 1)
				return;

			_coordinateSystemStack.Pop();
		}

		/// <summary>
		/// Draws the <see cref="Graphic"/>.
		/// </summary>
		public virtual void Draw()
		{
			Platform.CheckMemberIsSet(this.ParentPresentationImage, "PresentationImage");
			this.ParentPresentationImage.Draw();
		}

		/// <summary>
		/// Creates the <see cref="SpatialTransform"/> for this <see cref="Graphic"/>.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Override this if the default <see cref="SpatialTransform"/> created
		/// is not appropriate.
		/// </remarks>
		protected virtual SpatialTransform CreateSpatialTransform()
		{
			return new SpatialTransform(this);
		}
	}
}
