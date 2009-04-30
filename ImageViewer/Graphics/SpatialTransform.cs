#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Mathematics;
using Matrix=System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Graphics
{
	//TODO: the Matrix fields are not disposed although they are IDisposable

	/// <summary>
	/// Implements a 2D affine transformation
	/// </summary>
	[Cloneable]
	public class SpatialTransform : ISpatialTransform
	{
		#region Private fields

		private bool _updatingScaleParameters;
		private float _scale = 1.0f;
		private float _scaleX = 1.0f;
		private float _scaleY = 1.0f;
		private float _translationX;
		private float _translationY;
		private float _rotationXY;
		private bool _flipX;
		private bool _flipY;
		private PointF _centerOfRotationXY;
		private Matrix _cumulativeTransform;
		private Matrix _transform;
		private bool _recalculationRequired;
		[CloneIgnore]
		private IGraphic _ownerGraphic;
		private SpatialTransformValidationPolicy _validationPolicy;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="SpatialTransform"/>.
		/// </summary>
		public SpatialTransform(IGraphic ownerGraphic)
		{
			_ownerGraphic = ownerGraphic;
			_recalculationRequired = true;
			_updatingScaleParameters = false;
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected SpatialTransform(SpatialTransform source, ICloningContext context)
		{
			context.CloneFields(source, this);

			if (source._cumulativeTransform != null)
				_cumulativeTransform = source._cumulativeTransform.Clone();

			if (source._transform != null)
				_transform = source._transform.Clone();
		}

		#region Properties

		/// <summary>
		/// Gets whether or not we need to recalculate the <see cref="CumulativeTransform"/>.
		/// </summary>
		protected bool RecalculationRequired
		{
			get 
			{
				//check if the scale values have changed before returning.
				UpdateScaleInternal();

				if (_recalculationRequired)
				{
					return true;
				}
				else if (OwnerGraphic != null && OwnerGraphic.ParentGraphic != null)
				{
					//If something above us in the hierarchy needs recalculating, so do we.
					return OwnerGraphic.ParentGraphic.SpatialTransform.RecalculationRequired;
				}

				return false;
			}
			private set
			{
				if (value && OwnerGraphic is CompositeGraphic)
				{
					//If we need recalculation, so does everything below us.
					foreach (Graphic graphic in ((CompositeGraphic)OwnerGraphic).Graphics)
						graphic.SpatialTransform.ForceRecalculation();
				}

				_recalculationRequired = value;
			}
		}

		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		public float Scale
		{
			get
			{
				UpdateScaleInternal();
				return _scale;
			}
			set
			{
				if (_scale == value)
					return;

				if (value < 0 || FloatComparer.AreEqual(value, 0F))
					throw new ArgumentOutOfRangeException("Cannot set Scale to zero.");

				_scale = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the scale in the x-direction.
		/// </summary>
		/// <remarks>Usually, <see cref="Scale"/> = <see cref="ScaleX"/> = <see cref="ScaleY"/>.
		/// However, when pixels are non-square, <see cref="ScaleX"/> and <see cref="ScaleY"/>
		/// will differ.  Note that <see cref="ScaleX"/> does not account for flip and is
		/// thus always positive.</remarks>
		protected internal float ScaleX
		{
			get
			{
				UpdateScaleInternal();
				return _scaleX;
			}
			protected set 
			{
				if (_scaleX == value)
					return;

				if (value < 0 || FloatComparer.AreEqual(value, 0F))
					throw new ArgumentOutOfRangeException("Cannot set ScaleX to zero.");

				_scaleX = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the scale in the y-direction.
		/// </summary>
		/// <remarks>Usually, <see cref="Scale"/> = <see cref="ScaleX"/> = <see cref="ScaleY"/>.
		/// However, when pixels are non-square, <see cref="ScaleX"/> and <see cref="ScaleY"/>
		/// will differ.  Note that <see cref="ScaleY"/> does not account for flip and is
		/// thus always positive.</remarks>
		protected internal float ScaleY
		{
			get
			{
				UpdateScaleInternal();
				return _scaleY;
			}
			protected set 
			{
				if (_scaleY == value)
					return;

				if (value < 0 || FloatComparer.AreEqual(value, 0F))
					throw new ArgumentOutOfRangeException("Cannot set ScaleY to zero.");

				_scaleY = value;
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
				if (_translationX == value)
					return;
				
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
				if (_translationY == value)
					return;

				_translationY = value;
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
				if (_flipX == value)
					return;

				_flipX = value;
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
				if (_flipY == value)
					return;

				_flipY = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the rotation in the XY plane in degrees.
		/// </summary>
		/// <remarks>
		/// Values less than 0 or greater than 360 are converted to the equivalent
		/// angle between 0 and 360. For example, -5 becomes 355 and 390 becomes 30.
		/// </remarks>
		public int RotationXY
		{
			get { return (int)_rotationXY; }
			set 
			{
				value = value % 360;

				if (value < 0)
					value += 360;

				if (_rotationXY == value)
					return;

				_rotationXY = value;
				this.RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the center of rotation.
		/// </summary>
		/// <remarks>
		/// The point should be specified in terms of the coordinate system
		/// of the parent graphic, i.e. source coordinates.
		/// </remarks>
		public PointF CenterOfRotationXY
		{
			get { return _centerOfRotationXY; }
			set
			{
				if (_centerOfRotationXY == value)
					return;

				_centerOfRotationXY = value;
				this.RecalculationRequired = true;
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
				_transform.Scale(this.ScaleX * (this.FlipY ? -1 : 1), this.ScaleY * (this.FlipX ? -1 : 1));
				_transform.Translate(this.TranslationX, this.TranslationY);
				 
				return _transform;
			}
		}

		/// <summary>
		/// Gets the cumulative scale.
		/// </summary>
		/// <remarks>
		/// Gets the scale relative to the root of the scene graph.
		/// </remarks>
		public float CumulativeScale
		{
			get
			{
				float cumulativeScale = Scale;
				if (OwnerGraphic != null && OwnerGraphic.ParentGraphic != null)
					cumulativeScale *= OwnerGraphic.ParentGraphic.SpatialTransform.CumulativeScale;

				return cumulativeScale;
			}
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
				return _cumulativeTransform;
			}
		}

		/// <summary>
		/// Gets or sets the associated <see cref="SpatialTransformValidationPolicy"/>.
		/// </summary>
		/// <remarks>
		/// It is not always desirable to allow an <see cref="IGraphic"/> to be transformed
		/// in arbitrary ways.  For example, at present, images can only be rotated in
		/// 90 degree increments.  This property essentially allows a validation policy to be set on a
		/// per graphic basis.  If validation fails, an <see cref="ArgumentException"/> is thrown.
		/// </remarks>
		public SpatialTransformValidationPolicy ValidationPolicy
		{
			get { return _validationPolicy; }
			set { _validationPolicy = value; }
		}


		internal IGraphic OwnerGraphic
		{
			get { return _ownerGraphic; }
			set { _ownerGraphic = value; }
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
			ForceRecalculation();
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
		/// Converts a <see cref="SizeF"/> from source to destination coordinates.
		/// </summary>
		/// <remarks>
		/// Only scale and rotation are applied when converting sizes; this is equivalent
		/// to converting a direction vector, as direction vectors have only magnitude
		/// and direction information, but no position.
		/// </remarks>
		public SizeF ConvertToDestination(SizeF sourceDimensions)
		{
			PointF[] transformed = new PointF[] { sourceDimensions.ToPointF() };
			this.CumulativeTransform.TransformVectors(transformed);
			return new SizeF(transformed[0]);
		}

		/// <summary>
		/// Converts a <see cref="SizeF"/> from destination to source coordinates.
		/// </summary>
		/// <remarks>
		/// Only scale and rotation are applied when converting sizes; this is equivalent
		/// to converting a direction vector, as direction vectors have only magnitude
		/// and direction information, but no position.
		/// </remarks>
		public SizeF ConvertToSource(SizeF destinationDimensions)
		{
			PointF[] transformed = new PointF[] { destinationDimensions.ToPointF() };
			Matrix inverse = this.CumulativeTransform.Clone();
			inverse.Invert();
			inverse.TransformVectors(transformed);
			return new SizeF(transformed[0]);
		}

		#region IMemorable members

		/// <summary>
		/// Creates a memento for this object.
		/// </summary>
		/// <remarks>Typically used in conjunction with <see cref="MemorableUndoableCommand"/>
		/// to support undo/redo.</remarks>
		public virtual object CreateMemento()
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
		/// <remarks>Typically used in conjunction with <see cref="MemorableUndoableCommand"/>
		/// to support undo/redo.</remarks>
		/// <exception cref="ArgumentNullException"><b>memento</b>
		/// is <b>null</b>.</exception>
		/// <exception cref="InvalidCastException"><b>memento</b>
		/// is not of the type expected by the object.</exception>
		public virtual void SetMemento(object memento)
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

			this.RecalculationRequired = true;
		}

		#endregion

		#endregion

		#region Protected methods

		/// <summary>
		/// Forces the <see cref="CumulativeTransform"/> to be recalculated.
		/// </summary>
		internal protected void ForceRecalculation()
		{
			RecalculationRequired = true;
		}

		/// <summary>
		/// Calculates the cumulative transform.
		/// </summary>
		/// <remarks>Once this method is executed, the <see cref="CumulativeTransform"/>
		/// property will reflect any changes in the transform parameters.</remarks>
		protected virtual void Calculate()
		{
			if (!this.RecalculationRequired)
				return;

			// The cumulative transform is the product of the transform of the
			// parent graphic and the transform of this graphic (i.e. the current transform)
			// If there is no parent graphic, then the cumulative transform = current transform
			if (_cumulativeTransform == null)
				_cumulativeTransform = new Matrix();

			_cumulativeTransform.Reset();

			IGraphic parentGraphic = this.OwnerGraphic.ParentGraphic;

			if (parentGraphic != null)
				_cumulativeTransform.Multiply(parentGraphic.SpatialTransform.CumulativeTransform);

			CalculatePreTransform(_cumulativeTransform);
			_cumulativeTransform.Multiply(this.Transform);
			CalculatePostTransform(_cumulativeTransform);

			this.RecalculationRequired = false;

			// Validate if there's a validation policy in place.  Otherwise, assume all is good.
			if (_validationPolicy != null)
				_validationPolicy.Validate(this);
		}

		/// <summary>
		/// Gives subclasses an opportunity to perform a pre-transform transformation.
		/// </summary>
		/// <param name="cumulativeTransform"></param>
		protected virtual void CalculatePreTransform(Matrix cumulativeTransform)
		{
			cumulativeTransform.Translate(_centerOfRotationXY.X, _centerOfRotationXY.Y);
		}

		/// <summary>
		/// Gives subclasses an opportunity to perform a post-transform transformation.
		/// </summary>
		protected virtual void CalculatePostTransform(Matrix cumulativeTransform)
		{
			cumulativeTransform.Translate(-_centerOfRotationXY.X, -_centerOfRotationXY.Y);
		}

		/// <summary>
		/// Gives derived classes an opportunity to update the scale parameters.
		/// </summary>
		/// <remarks>
		/// By default, sets <see cref="ScaleX"/> and <see cref="ScaleY"/> to the value of <see cref="Scale"/>.
		/// </remarks>
		protected virtual void UpdateScaleParameters()
		{
			float scale = Scale;
			this.ScaleX = scale;
			this.ScaleY = scale;
		}

		private void UpdateScaleInternal()
		{
			if (_updatingScaleParameters)
				return;

			_updatingScaleParameters = true;
			UpdateScaleParameters();
			_updatingScaleParameters = false;
		}

		#endregion
	}
}
