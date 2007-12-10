#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Implements a 2D affine transformation
	/// </summary>
	public class SpatialTransform : ISpatialTransform
	{
		#region Private fields

		private float _scale = 1.0f;
		private float _maximumScale = 64.0f;
		private float _minimumScale = 0.25f;
		private float _scaleX = 1.0f;
		private float _scaleY = 1.0f;
		private float _cumulativeScale = 1.0f;
		private float _translationX;
		private float _translationY;
		private float _rotationXY;
		private bool _flipX;
		private bool _flipY;
		private Matrix _cumulativeTransform;
		private Matrix _transform;
		private Rectangle _clientRectangle;
		private bool _recalculationRequired;
		private IGraphic _ownerGraphic;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="SpatialTransform"/>.
		/// </summary>
		public SpatialTransform(IGraphic ownerGraphic)
		{
			_ownerGraphic = ownerGraphic;
			Initialize();
		}

		#region Properties

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

				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the minimum allowable scale.
		/// </summary>
		public float MinimumScale
		{
			get { return _minimumScale; }
			protected set { _minimumScale = value; }
		}

		/// <summary>
		/// Gets or sets the maximum allowable scale.
		/// </summary>
		public float MaximumScale
		{
			get { return _maximumScale; }
			protected set { _maximumScale = value; }
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
		/// Gets or sets a value indicating whether images are flipped horizontally
		/// (i.e., the y-axis as the axis of reflection)
		/// </summary>
		public bool FlipY
		{
			get { return _flipY; }
			set 
			{ 
				_flipY = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether images are flipped vertically
		/// (i.e., the x-axis as the axis of reflection)
		/// </summary>
		public bool FlipX
		{
			get { return _flipX; }
			set 
			{ 
				_flipX = value;
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
		/// <exception cref="ArgumentException">The rotation is not a multiple
		/// of 90 degrees.</exception>
		public int RotationXY
		{
			get { return (int)_rotationXY; }
			set 
			{
				if ((value % 90) != 0)
					throw new ArgumentException(SR.ExceptionInvalidRotationValue);

				_rotationXY = value % 360;

				if (_rotationXY < 0)
					_rotationXY += 360;

				this.RecalculationRequired = true;
			}
		}


		/// <summary>
		/// Gets or sets the scale in the x-direction.
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
		/// Gets or sets the scale in the y-direction.
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

		/// <summary>
		/// Gets the transform relative to the <see cref="IGraphic"/> object's
		/// immediate parent <see cref="IGraphic"/>.
		/// </summary>
		public Matrix Transform
		{
			get
			{
				if (_transform == null)
					_transform = new Matrix();

				_transform.Reset();
				_transform.Rotate(this.RotationXY);
				_transform.Scale(this.ScaleX, this.ScaleY);
				_transform.Translate(this.TranslationX, this.TranslationY);
				 
				return _transform;
			}
		}

		/// <summary>
		/// Gets the cumulative scale.
		/// </summary>
		/// <remarks>
		/// This is the scale relative to the root of the scene graph.
		/// </remarks>
		public float CumulativeScale
		{
			get { return _cumulativeScale; }
			protected set { _cumulativeScale = value; }
		}

		/// <summary>
		/// Gets the cumulative transform.
		/// </summary>
		/// <remarks>
		/// The <see cref="CumulativeTransform"/> is the product of an
		/// <see cref="IGraphic"/>'s <see cref="Transform"/> and the
		/// <see cref="Transform"/> of all of nodes above it (i.e., all of its
		/// ancestors).
		/// </remarks>
		public Matrix CumulativeTransform
		{
			get 
			{
				Calculate();
				return this.CumulativeTransformInternal; 
			}
		}

#if UNIT_TESTS
		/// <summary>
		/// Gets the client rectangle for this <see cref="SpatialTransform"/>.
		/// </summary>
		public Rectangle ClientRectangle
#else
		internal Rectangle ClientRectangle
#endif
		{
			get { return _clientRectangle; }
			set 
			{ 
				_clientRectangle = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the transformation
		/// needs to be recalculated.
		/// </summary>
		protected bool RecalculationRequired
		{
			get { return _recalculationRequired; }
			set { _recalculationRequired = value; }
		}

		private Matrix CumulativeTransformInternal
		{
			get
			{
				if (_cumulativeTransform == null)
					_cumulativeTransform = new Matrix();

				return _cumulativeTransform;
			}
		}

		internal IGraphic OwnerGraphic
		{
			get { return _ownerGraphic; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Resets all transform parameters to their defaults.
		/// </summary>
		public void Initialize()
		{
			this.Scale = 1.0F;
			this.TranslationX = 0.0F;
			this.TranslationY = 0.0F;
			this.RotationXY = 0;
			this.FlipY = false;
			this.FlipX = false;
		}

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
		/// Converts an array of vectors from source to destination coordinates.
		/// </summary>
		/// <remarks>
		/// The input array is modified directly, and contains the return values.
		/// </remarks>
		public void ConvertVectorsToDestination(PointF[] sourceVectors)
		{
			this.CumulativeTransform.TransformVectors(sourceVectors);
		}

		/// <summary>
		/// Converts an array of vectors from destination to source coordinates.
		/// </summary>
		/// <remarks>
		/// The input array is modified directly, and contains the return values.
		/// </remarks>
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

		#region IMemorable members

		/// <summary>
		/// Creates a memento for this object.
		/// </summary>
		/// <remarks>Typically used in conjunction with <see cref="UndoableCommand"/>
		/// to support undo/redo.</remarks>
		public virtual IMemento CreateMemento()
		{
			SpatialTransformMemento memento = new SpatialTransformMemento();

			memento.FlipX = this.FlipX;
			memento.FlipY = this.FlipY;
			memento.RotationXY = this.RotationXY;
			memento.Scale = this.Scale;
			memento.TranslationX = this.TranslationX;
			memento.TranslationY = this.TranslationY;

			return memento;
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
		public virtual void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			SpatialTransformMemento spatialTransformMemento = memento as SpatialTransformMemento;
			Platform.CheckForInvalidCast(spatialTransformMemento, "memento", "SpatialTransformMemento");

			this.FlipX = spatialTransformMemento.FlipX;
			this.FlipY = spatialTransformMemento.FlipY;
			this.RotationXY = spatialTransformMemento.RotationXY;
			this.Scale = spatialTransformMemento.Scale;
			this.TranslationX = spatialTransformMemento.TranslationX;
			this.TranslationY = spatialTransformMemento.TranslationY;
		}

		#endregion

		#endregion

		#region Protected methods

		/// <summary>
		/// Calculates the cumulative transform.
		/// </summary>
		/// <remarks>Once this method is executed, the <see cref="CumulativeTransform"/>
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

			IGraphic parentGraphic = this.OwnerGraphic.ParentGraphic;

			if (parentGraphic != null)
				this.CumulativeTransformInternal.Multiply(parentGraphic.SpatialTransform.CumulativeTransform);

			CalculatePreTransform(this.CumulativeTransformInternal);
			this.CumulativeTransformInternal.Multiply(this.Transform);
			CalculatePostTransform(this.CumulativeTransformInternal);

			this.CumulativeScale = this.Scale;

			if (parentGraphic != null)
				this.CumulativeScale *= parentGraphic.SpatialTransform.CumulativeScale;

			this.RecalculationRequired = false;
		}

		/// <summary>
		/// Gives subclasses an opportunity to perform a pre-transform transformation.
		/// </summary>
		/// <param name="cumulativeTransform"></param>
		protected virtual void CalculatePreTransform(Matrix cumulativeTransform)
		{
		}

		/// <summary>
		/// Gives subclasses an opportunity to perform a post-transform transformation.
		/// </summary>
		protected virtual void CalculatePostTransform(Matrix cumulativeTransform)
		{
		}

		/// <summary>
		/// Gives subclasses an opportunity to override the scale calculation.
		/// </summary>
		protected virtual void CalculateScale()
		{
			this.ScaleX = _scale;
			this.ScaleY = _scale;
		}

		/// <summary>
		/// Gives subclasses an opportunity to override the flip calculation.
		/// </summary>
		protected virtual void CalculateFlip()
		{
			if (_flipX)
				this.ScaleY *= -1;

			if (_flipY)
				this.ScaleX *= -1;
		}

		#endregion
	}
}
