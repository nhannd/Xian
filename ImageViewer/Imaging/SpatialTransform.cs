using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements a 2D affine transformation
	/// </summary>
	public class SpatialTransform : ISpatialTransform
	{
		// Private attributes
		private float _scale = 1.0f;
		private float _maximumScale = 64.0f;
		private float _minimumScale = 0.25f;
		private float _scaleX = 1.0f;
		private float _scaleY = 1.0f;
		private bool _scaleToFit;
		private float _cumulativeScale = 1.0f;
		private float _translationX;
		private float _translationY;
		private float _rotation;
		private bool _flipHorizontal;
		private bool _flipVertical;
		private Matrix _cumulativeTransform;
		private Matrix _transform;
		private Rectangle _clientRectangle;
		private bool _recalculationRequired;
		private Graphic _ownerGraphic;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpatialTransform"/> class.
		/// </summary>
		public SpatialTransform(Graphic ownerGraphic)
		{
			_ownerGraphic = ownerGraphic;
			Initialize();
		}

		internal Graphic OwnerGraphic
		{
			get { return _ownerGraphic; }
		}

		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		public float Scale
		{
			get { return _scale; }
			set
			{
				if (value > _maximumScale)
					_scale = _maximumScale;
				else if (value < _minimumScale)
					_scale = _minimumScale;
				else
					_scale = value;

				this.ScaleX = _scale;
				this.ScaleY = _scale;

				this.RecalculationRequired = true;
			}
		}

		public float MinimumScale
		{
			get { return _minimumScale; }
			protected set { _minimumScale = value; }
		}

		public float MaximumScale
		{
			get { return _maximumScale; }
			protected set { _maximumScale = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether images will be scaled to fit
		/// in a <see cref="Tile"/>.
		/// </summary>
		public bool ScaleToFit
		{
			get { return _scaleToFit; }
			set
			{
				_scaleToFit = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the translation along the x-axis.
		/// </summary>
		public float TranslationX
		{
			get { return _translationX; }
			set	
			{ 
				_translationX = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the translation along the y-axis.
		/// </summary>
		public float TranslationY
		{
			get	{ return _translationY; }
			set	
			{ 
				_translationY = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether images will flipped horizontally
		/// (i.e., the y-axis as the axis of reflection)
		/// </summary>
		public bool FlipHorizontal
		{
			get { return _flipHorizontal; }
			set 
			{ 
				_flipHorizontal = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether images will flipped vertically
		/// (i.e., the x-axis as the axis of reflection)
		/// </summary>
		public bool FlipVertical
		{
			get { return _flipVertical; }
			set 
			{ 
				_flipVertical = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the rotation in degrees.
		/// </summary>
		/// <remarks>
		/// Any multiple of 90 is allowed.  However, the value will always be "wrap" to
		/// either 0, 90, 180 or 270.  For example, if set to -450, the property will
		/// return 270.
		/// </remarks>
		public int Rotation
		{
			get { return (int)_rotation; }
			set 
			{
				if ((value % 90) != 0)
					throw new ArgumentException(SR.ExceptionInvalidRotationValue);

				_rotation = value % 360;

				if (_rotation < 0)
					_rotation += 360;

				this.RecalculationRequired = true;
			}
		}


		/// <summary>
		/// Gets the scale in the x-direction.
		/// </summary>
		/// <remarks>Usually, <see cref="Scale"/> = <see cref="ScaleX"/> = <see cref="ScaleY"/>.
		/// However, when pixels are non-square, <see cref="ScaleX"/> and <see cref="ScaleY"/>
		/// will differ.</remarks>
		public float ScaleX
		{
			get { return _scaleX; }
			protected set 
			{ 
				_scaleX = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets the scale in the y-direction.
		/// </summary>
		/// <remarks>Usually, <see cref="Scale"/> = <see cref="ScaleX"/> = <see cref="ScaleY"/>.
		/// However, when pixels are non-square, <see cref="ScaleX"/> and <see cref="ScaleY"/>
		/// will differ.</remarks>
		public float ScaleY
		{
			get { return _scaleY; }
			protected set 
			{
				this.RecalculationRequired = true;
				_scaleY = value; 
			}
		}

		public Matrix Transform
		{
			get
			{
				if (_transform == null)
					_transform = new Matrix();

				_transform.Reset();
				_transform.Translate(this.TranslationX * Math.Abs(this.ScaleX), this.TranslationY * Math.Abs(this.ScaleY));
				_transform.Scale(this.ScaleX, this.ScaleY);
				_transform.Rotate(this.Rotation);

				return _transform;
			}
		}

		public float CumulativeScale
		{
			get { return _cumulativeScale; }
			protected set { _cumulativeScale = value; }
		}

		public Matrix CumulativeTransform
		{
			get 
			{
				Calculate();
				return this.CumulativeTransformInternal; 
			}
		}

		protected Matrix CumulativeTransformInternal
		{
			get
			{
				if (_cumulativeTransform == null)
					_cumulativeTransform = new Matrix();

				return _cumulativeTransform;
			}
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			set 
			{ 
				_clientRectangle = value;
				this.RecalculationRequired = true;
			}
		}

		protected bool RecalculationRequired
		{
			get { return _recalculationRequired; }
			set { _recalculationRequired = value; }
		}

		/// <summary>
		/// Resets all transform parameters to their defaults.
		/// </summary>
		public virtual void Initialize()
		{
			this.Scale = 1.0F;
			this.ScaleX = 1.0F;
			this.ScaleY = 1.0F;
			this.ScaleToFit = true;
			this.TranslationX = 0.0F;
			this.TranslationY = 0.0F;
			this.Rotation = 0;
			this.FlipHorizontal = false;
			this.FlipVertical = false;
		}

		/// <summary>
		/// Calculates the transform.
		/// </summary>
		/// <remarks>Once this method is executed, the <see cref="Transform"/>
		/// property will reflect any changes in the transform parameters.</remarks>
		protected virtual void Calculate()
		{
			if (!this.RecalculationRequired)
				return;

			CalculateScale();
			CalculateFlip();

			// The cumulative transform is the product of the transform of the
			// parent graphic and the transform of this graphic (i.e. the current transform)
			// If there is no parent graphic, then the cumulative transform = current transform
			this.CumulativeTransformInternal.Reset();

			Graphic parentGraphic = this.OwnerGraphic.ParentGraphic as Graphic;

			if (parentGraphic != null)
				this.CumulativeTransformInternal.Multiply(parentGraphic.SpatialTransform.CumulativeTransform);

			CalculatePreTransform();
			this.CumulativeTransformInternal.Multiply(this.Transform);
			CalculatePostTransform();

			this.CumulativeScale = this.Scale;

			if (parentGraphic != null)
				this.CumulativeScale *= parentGraphic.SpatialTransform.CumulativeScale;

			this.RecalculationRequired = false;
		}

		protected virtual void CalculatePreTransform()
		{
		}

		protected virtual void CalculatePostTransform()
		{
		}


		#region IMemorable members

		/// <summary>
		/// Creates a memento for this object.
		/// </summary>
		/// <remarks>Typically used in conjunction with <see cref="UndoableCommand"/>
		/// to support undo/redo.</remarks>
		public IMemento CreateMemento()
		{
			SpatialTransformMemento spatialTransformMemento = new SpatialTransformMemento();

			spatialTransformMemento.FlipHorizontal = this.FlipHorizontal;
			spatialTransformMemento.FlipVertical = this.FlipVertical;
			spatialTransformMemento.Rotation = this.Rotation;
			spatialTransformMemento.Scale = this.Scale;
			//spatialTransformMemento.ScaleToFit = this.ScaleToFit;
			spatialTransformMemento.TranslationX = this.TranslationX;
			spatialTransformMemento.TranslationY = this.TranslationY;

			return spatialTransformMemento as IMemento;
		}

		/// <summary>
		/// Sets a memento for this object.
		/// </summary>
		/// <remarks>Typically used in conjunction with <see cref="UndoableCommand"/>
		/// to support undo/redo.</remarks>
		/// <exception cref="ArgumentNullException"><b>memento</b>
		/// is <b>null</b>.</exception>
		/// <exception cref="InvalidCastException"><b>memento</b>
		/// is not of the type expected by the object.</exception>
		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			SpatialTransformMemento spatialTransformMemento = memento as SpatialTransformMemento;
			Platform.CheckForInvalidCast(spatialTransformMemento, "memento", "PresentationViewMemento");

			this.FlipHorizontal = spatialTransformMemento.FlipHorizontal;
			this.FlipVertical = spatialTransformMemento.FlipVertical;
			this.Rotation = spatialTransformMemento.Rotation;
			this.Scale = spatialTransformMemento.Scale;
			this.ScaleToFit = spatialTransformMemento.ScaleToFit;
			this.TranslationX = spatialTransformMemento.TranslationX;
			this.TranslationY = spatialTransformMemento.TranslationY;
		}

		#endregion

		/// <summary>
		/// Converts a <see cref="PointF"/> from source to destination coordinates.
		/// </summary>
		/// <param name="sourcePoint"></param>
		/// <returns></returns>
		public PointF ConvertToDestination(PointF sourcePoint)
		{
			PointF[] points = new PointF[1];
			points[0] = sourcePoint;
			this.CumulativeTransform.TransformPoints(points);
			
			return points[0];
		}

		/// <summary>
		/// Converts a <see cref="PointF"/> from destination to source coordinates.
		/// </summary>
		/// <param name="destinationPoint"></param>
		/// <returns></returns>
		public PointF ConvertToSource(PointF destinationPoint)
		{
			Matrix inverse = this.CumulativeTransform.Clone();
			inverse.Invert();

			PointF[] points = new PointF[1];
			points[0] = destinationPoint;
			inverse.TransformPoints(points);
			
			return points[0];
		}

		/// <summary>
		/// Converts a <see cref="RectangleF"/> from source to destination coordinates.
		/// </summary>
		/// <param name="sourceRectangle"></param>
		/// <returns></returns>
		public RectangleF ConvertToDestination(RectangleF sourceRectangle)
		{
			PointF topLeft = ConvertToDestination(sourceRectangle.Location);
			PointF bottomRight = ConvertToDestination(new PointF(sourceRectangle.Right, sourceRectangle.Bottom));

			return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
		}

		/// <summary>
		/// Converts a <see cref="RectangleF"/> from destination to source coordinates.
		/// </summary>
		/// <param name="destinationRectangle"></param>
		/// <returns></returns>
		public RectangleF ConvertToSource(RectangleF destinationRectangle)
		{
			PointF topLeft = ConvertToSource(destinationRectangle.Location);
			PointF bottomRight = ConvertToSource(new PointF(destinationRectangle.Right, destinationRectangle.Bottom));

			return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
		}

		/// <summary>
		/// Transforms an array of vectors from the source to destination coordinate system.
		/// The input array is modified directly, and contains the return values.
		/// </summary>
		public void ConvertVectorsToDestination(PointF[] sourceVectors)
		{
			this.CumulativeTransform.TransformVectors(sourceVectors);
		}

		/// <summary>
		/// Transforms an array of vectors from the destination to source coordinate system.
		/// The input array is modified directly, and contains the return values.
		/// </summary>
		public void ConvertVectorsToSource(PointF[] destinationVectors)
		{
			Matrix inverse = this.CumulativeTransform.Clone();
			inverse.Invert();

			inverse.TransformVectors(destinationVectors);
		}

		/// <summary>
		/// Converts a <see cref="SizeF"/> from source to destination coordinates.
		/// </summary>
		/// <param name="sourceDimensions"></param>
		/// <returns></returns>
		public SizeF ConvertToDestination(SizeF sourceDimensions)
		{
			float width = sourceDimensions.Width * this.ScaleX;
			float height = sourceDimensions.Height * this.ScaleY;

			return new SizeF(width, height);
		}

		/// <summary>
		/// Converts a <see cref="SizeF"/> from destination to source coordinates.
		/// </summary>
		/// <param name="destinationDimensions"></param>
		/// <returns></returns>
		public SizeF ConvertToSource(SizeF destinationDimensions)
		{
			float width = destinationDimensions.Width / this.ScaleX;
			float height = destinationDimensions.Height / this.ScaleY;

			return new SizeF(width, height);
		}

		protected virtual void CalculateScale()
		{
		}

		protected void CalculateFlip()
		{
			if (_flipVertical)
				this.ScaleY *= -1;

			if (_flipHorizontal)
				this.ScaleX *= -1;
		}
	}
}
