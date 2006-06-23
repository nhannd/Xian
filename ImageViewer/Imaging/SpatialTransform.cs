using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements a 2D affine transformation
	/// </summary>
	public class SpatialTransform : IMemorable
	{
		// Private attributes
		private Rectangle _destinationRectangle;
		private Rectangle _sourceRectangle;
		private double _pixelSpacingX;
		private double _pixelSpacingY;
		private int _pixelAspectRatioX;
		private int _pixelAspectRatioY;
		private bool _scaleToFit;
		private float _scale;
		private float _maximumScale = 64.0F;
		private float _minimumScale = 0.5F;
		private float _scaleX;
		private float _scaleY;
		private float _translationX;
		private float _translationY;
		private float _rotation;
		private bool _flipHorizontal;
		private bool _flipVertical;
		private Matrix _resultMatrix = new Matrix();
		private bool _recalculationRequired;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpatialTransform"/> class.
		/// </summary>
		public SpatialTransform()
		{
			Initialize();
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
				_recalculationRequired = true;
			}
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

				_recalculationRequired = true;
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
				_recalculationRequired = true;
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
				_recalculationRequired = true;
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
				_recalculationRequired = true;
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
				_recalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the rotation in degrees.
		/// </summary>
		public float Rotation
		{
			get { return _rotation; }
			set 
			{ 
				_rotation = value;
				_recalculationRequired = true;
			}
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
			spatialTransformMemento.ScaleToFit = this.ScaleToFit;
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

			Calculate();
		}

		#endregion

		/// <summary>
		/// Gets the <see cref="Matrix"/> representing the transform.
		/// </summary>
		public Matrix Transform
		{
			get { return _resultMatrix; }
		}

		/// <summary>
		/// Gets or sets the destination rectangle.
		/// </summary>
		/// <remarks>Typically, this would be the rectangle of the
		/// <see cref="Tile"/> in which layers are being rendered.
		/// The top left corner of the <see cref="Tile"/> corresponds to
		/// (0,0).</remarks>
		/// <exception cref="ArgumentNullException"><i>value</i> is <b>null</b></exception>
		public Rectangle DestinationRectangle
		{
			get { return _destinationRectangle; }
			set 
			{
				Platform.CheckForNullReference(value, "DestinationRectangle");
				_destinationRectangle = value;
				_recalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the source rectangle.
		/// </summary>
		/// <remarks>Typically, this would be the rectangle of the
		/// original image being rendered.</remarks>
		/// <exception cref="ArgumentNullException"><i>value</i> is <b>null</b></exception>
		public Rectangle SourceRectangle
		{
			get { return _sourceRectangle; }
			set 
			{
				Platform.CheckForNullReference(value, "SourceRectangle");
				_sourceRectangle = value;
				_recalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the pixel spacing in the x-direction.
		/// </summary>
		public double PixelSpacingX
		{
			get { return _pixelSpacingX; }
			set	
			{ 
				_pixelSpacingX = value;
				_recalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the pixel spacing in the y-direction.
		/// </summary>
		public double PixelSpacingY
		{
			get { return _pixelSpacingY; }
			set	
			{ 
				_pixelSpacingY = value;
				_recalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the pixel aspect ratio in the x-direction.
		/// </summary>
		public int PixelAspectRatioX
		{
			get { return _pixelAspectRatioX; }
			set	
			{ 
				_pixelAspectRatioX = value;
				_recalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the pixel aspect ratio in the y-direction.
		/// </summary>
		public int PixelAspectRatioY
		{
			get { return _pixelAspectRatioY; }
			set	
			{ 
				_pixelAspectRatioY = value;
				_recalculationRequired = true;
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
		}

		/// <summary>
		/// Gets the scale in the y-direction.
		/// </summary>
		/// <remarks>Usually, <see cref="Scale"/> = <see cref="ScaleX"/> = <see cref="ScaleY"/>.
		/// However, when pixels are non-square, <see cref="ScaleX"/> and <see cref="ScaleY"/>
		/// will differ.</remarks>
		public float ScaleY
		{
			get	{ return _scaleY; }
		}

		/// <summary>
		/// Resets all transform parameters to their defaults.
		/// </summary>
		public void Initialize()
		{
			_pixelSpacingX = 0.0d;
			_pixelSpacingY = 0.0d;
			_pixelAspectRatioX = 0;
			_pixelAspectRatioY = 0;
			_scaleToFit = true;
			_scale = 1.0F;
			_scaleX = 1.0F;
			_scaleY = 1.0F;
			_translationX = 0.0F;
			_translationY = 0.0F;
			_rotation = 0.0F;
			_flipHorizontal = false;
			_flipVertical = false;
			_recalculationRequired = true;
		}

		/// <summary>
		/// Calculates the transform.
		/// </summary>
		/// <remarks>Once this method is executed, the <see cref="Transform"/>
		/// property will reflect any changes in the transform parameters.</remarks>
		public void Calculate()
		{
			if (!_recalculationRequired)
				return;

			Platform.CheckForNullReference(_sourceRectangle, "m_SourceRectangle");
			Platform.CheckForNullReference(_destinationRectangle, "m_DestinationRectangle");

			// Don't bother calculating anything if the area of either rectangle is zero
			if (_sourceRectangle.Width == 0 || _sourceRectangle.Height == 0 ||
				_destinationRectangle.Width == 0 || _destinationRectangle.Height == 0)
				return;

			CalculateScale();
			CalculateFlip();

			Matrix scaleMatrix = new Matrix();
			scaleMatrix.Scale(_scaleX, _scaleY);

			Matrix translateCenterTileMatrix = new Matrix();
			translateCenterTileMatrix.Translate(_destinationRectangle.Left + _destinationRectangle.Width / 2.0f, _destinationRectangle.Top + _destinationRectangle.Height / 2.0f);

			Matrix translateCenterImageMatrix = new Matrix();
			translateCenterImageMatrix.Translate(-(_sourceRectangle.Width / 2.0f) * _scaleX, -(_sourceRectangle.Height / 2.0f) * _scaleY);

			Matrix translateOffsetMatrix = new Matrix();
			translateOffsetMatrix.Translate(_translationX * Math.Abs(_scaleX), _translationY * Math.Abs(_scaleY));

			Matrix rotationMatrix = new Matrix();
			rotationMatrix.Rotate(_rotation);

			_resultMatrix.Reset();
			_resultMatrix.Multiply(translateCenterTileMatrix);
			_resultMatrix.Multiply(translateOffsetMatrix);
			_resultMatrix.Multiply(rotationMatrix);
			_resultMatrix.Multiply(translateCenterImageMatrix);
			_resultMatrix.Multiply(scaleMatrix);

			_recalculationRequired = false;
		}

		/// <summary>
		/// Converts a <see cref="PointF"/> from source to destination coordinates.
		/// </summary>
		/// <param name="sourcePoint"></param>
		/// <returns></returns>
		public PointF ConvertToDestination(PointF sourcePoint)
		{
			Calculate();

			PointF[] points = new PointF[1];
			points[0] = sourcePoint;
			_resultMatrix.TransformPoints(points);
			
			return points[0];
		}

		/// <summary>
		/// Converts a <see cref="PointF"/> from destination to source coordinates.
		/// </summary>
		/// <param name="destinationPoint"></param>
		/// <returns></returns>
		public PointF ConvertToSource(PointF destinationPoint)
		{
			Calculate();

			Matrix inverse = _resultMatrix.Clone();
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
		/// Converts a <see cref="SizeF"/> from source to destination coordinates.
		/// </summary>
		/// <param name="sourceDimensions"></param>
		/// <returns></returns>
		public SizeF ConvertToDestination(SizeF sourceDimensions)
		{
			float width = sourceDimensions.Width * _scaleX;
			float height = sourceDimensions.Height * _scaleY;

			return new SizeF(width, height);
		}

		/// <summary>
		/// Converts a <see cref="SizeF"/> from destination to source coordinates.
		/// </summary>
		/// <param name="destinationDimensions"></param>
		/// <returns></returns>
		public SizeF ConvertToSource(SizeF destinationDimensions)
		{
			float width = destinationDimensions.Width / _scaleX;
			float height = destinationDimensions.Height / _scaleY;

			return new SizeF(width, height);
		}

		// Private methods
		private void CalculateScale()
		{
			float pixelAspectRatio;
			
			if (_pixelAspectRatioX == 0 || _pixelAspectRatioY == 0)
			{
				if (_pixelSpacingX == 0 || _pixelSpacingY == 0)
					pixelAspectRatio = 1;
				else
					pixelAspectRatio = (float) _pixelSpacingY / (float) _pixelSpacingX;
			}
			else
			{
				pixelAspectRatio = (float) _pixelAspectRatioY / (float) _pixelAspectRatioX;
			}

			if (_scaleToFit)
				CalculateScaleToFit();

			if (pixelAspectRatio >= 1)
			{
				_scaleX = _scale * pixelAspectRatio;
				_scaleY = _scale;
			}
			else
			{
				_scaleX = _scale;
				_scaleY = _scale / pixelAspectRatio;
			}
		}

		private void CalculateScaleToFit()
		{
			if (_rotation == 90 || _rotation == 270)
			{
				float imageAspectRatio = (float) _sourceRectangle.Width / (float) _sourceRectangle.Height;
				float clientAspectRatio = (float) _destinationRectangle.Height / (float) _destinationRectangle.Width;

				if (clientAspectRatio >= imageAspectRatio)
					_scale = (float) _destinationRectangle.Width / (float) _sourceRectangle.Height;
				else
					_scale = (float) _destinationRectangle.Height / (float) _sourceRectangle.Width;
			}
			else
			{
				float imageAspectRatio = (float) _sourceRectangle.Height / (float) _sourceRectangle.Width;
				float clientAspectRatio = (float) _destinationRectangle.Height / (float) _destinationRectangle.Width;

				if (clientAspectRatio >= imageAspectRatio)
					_scale = (float) _destinationRectangle.Width / (float) _sourceRectangle.Width;
				else
					_scale = (float) _destinationRectangle.Height / (float) _sourceRectangle.Height;
			}

			_minimumScale = _scale / 2;
		}

		private void CalculateFlip()
		{
			if (_flipVertical)
				_scaleY *= -1;

			if (_flipHorizontal)
				_scaleX *= -1;
		}
	}
}
