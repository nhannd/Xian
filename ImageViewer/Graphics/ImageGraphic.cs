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
using ClearCanvas.Dicom.Validation;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An <see cref="InterpolationMode"/> enumeration specifies the
	/// interpolation algorithm to use when rendering the image.
	/// </summary>
	public enum InterpolationMode 
	{ 
		/// <summary>
		/// Specifies bilinear interpolation using fixed-point arithmetic.
		/// </summary>
		Bilinear 
	};

	/// <summary>
	/// An image <see cref="Graphic"/>.
	/// </summary>
	/// <remarks>
	/// The derived classes <see cref="GrayscaleImageGraphic"/> and 
	/// <see cref="ColorImageGraphic"/> represent the two basic types of
	/// 2D images in the framework.
	/// 
	/// An <see cref="ImageGraphic"/> is always a leaf in the scene graph.
	/// </remarks>
	[Cloneable]
	public abstract class ImageGraphic : Graphic
	{
		#region Private fields

		private int _rows;
		private int _columns;
		private int _bitsPerPixel;

		[CloneCopyReference]
		private byte[] _pixelDataRaw;
		[CloneCopyReference]
		private PixelDataGetter _pixelDataGetter;
		private PixelData _pixelDataWrapper;

		private int _sizeInBytes = -1;
		private int _sizeInPixels = -1;
		private int _doubleWordAlignedColumns = -1;

		private InterpolationMode _interpolationMode = InterpolationMode.Bilinear;

		#endregion

		#region Protected constructor

		/// <summary>
		/// Initializes a new instance of <see cref="ImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsPerPixel">Can be 8 or 16 in the case of
		/// grayscale images, or 32 for multichannel colour images.</param>
		/// <remarks>
		/// Creates an empty image of a specific size and bit depth.
		/// All all entries in the byte buffer are set to zero. Useful as
		/// a canvas on which pixels can be set by the client.
		/// </remarks>
		/// <exception cref="ArgumentException"><paramref name="rows"/> or
		/// <paramref name="columns"/> is negative, or <paramref name="bitsPerPixel"/>
		/// is not one of 8, 16 or 32.</exception>
		protected ImageGraphic(int rows, int columns, int bitsPerPixel)
			: this(rows, columns, bitsPerPixel, AllocatePixelData(rows, columns, bitsPerPixel))
		{
		}

		private static byte[] AllocatePixelData(int rows, int columns, int bitsPerPixel)
		{
			return MemoryManager.Allocate<byte>(rows*columns*bitsPerPixel/8);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageGraphic"/>		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsPerPixel">Can be 8 or 16 in the case of
		/// grayscale images, or 32 for multichannel colour images.</param>
		/// <param name="pixelData"></param>
		/// <remarks>
		/// Creates an image using existing pixel data.
		/// </remarks>
		/// <exception cref="NullReferenceException"><paramref name="pixelData"/> is <b>null</b></exception>
		/// <exception cref="ArgumentException"><paramref name="rows"/> or
		/// <paramref name="columns"/> is negative, or <paramref name="bitsPerPixel"/>
		/// is not one of 8, 16 or 32.</exception>
		protected ImageGraphic(int rows, int columns, int bitsPerPixel, byte[] pixelData)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");
			DicomValidator.ValidatePixelData(pixelData, rows, columns, bitsPerPixel);
			_pixelDataRaw = pixelData;
			Initialize(rows, columns, bitsPerPixel);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsPerPixel">Can be 8 or 16 in the case of
		/// grayscale images, or 32 for multichannel colour images.</param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// Creates an image using existing pixel data but does so
		/// without ever storing a reference to the pixel data. This is necessary
		/// to ensure that pixel data can be properly garbage collected in
		/// any future memory management schemes.
		/// </remarks>
		protected ImageGraphic(int rows, int columns, int bitsPerPixel, PixelDataGetter pixelDataGetter)
		{
			Platform.CheckForNullReference(pixelDataGetter, "pixelDataGetter");
			_pixelDataGetter = pixelDataGetter;
			Initialize(rows, columns, bitsPerPixel);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected ImageGraphic(ImageGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
			SetValidationPolicy();
		}

		private void Initialize(int rows, int columns, int bitsPerPixel)
		{
			DicomValidator.ValidateRows(rows);
			DicomValidator.ValidateColumns(columns);

			_rows = rows;
			_columns = columns;
			_bitsPerPixel = bitsPerPixel;
			SetValidationPolicy();
		}

		private void SetValidationPolicy()
		{
			if (this.SpatialTransform.ValidationPolicy is ImageTransformPolicy)
				return;
			
			this.SpatialTransform.ValidationPolicy = new ImageTransformPolicy();
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets the number of rows in the image.
		/// </summary>
		public int Rows 
		{ 
			get { return _rows; } 
		}

		/// <summary>
		/// Gets the number of columns in the image.
		/// </summary>
		public int Columns 
		{
			get { return _columns; } 
		}

		/// <summary>
		/// Gets the number of bits per pixel.
		/// </summary>
		/// <remarks>In the case of <see cref="GrayscaleImageGraphic"/>, this
		/// property will always have a value of 8 or 16, whereas in the
		/// case of <see cref="ColorImageGraphic"/>, it will always have
		/// a value of 32 (ARGB).</remarks>
		public int BitsPerPixel 
		{
			get { return _bitsPerPixel; }
		}

		/// <summary>
		/// Gets the number of bytes per pixel.
		/// </summary>
		public int BytesPerPixel
		{
			get { return this.BitsPerPixel/8; }
		}

		/// <summary>
		/// Gets a value indicating whether image is aligned on a 4-byte boundary
		/// </summary>
		/// <remarks>Bitmaps in Windows need to be aligned on a 4-byte boundary.  
		/// That is, the width of an image must be divisible by 4.</remarks>
		public bool IsDoubleWordAligned
		{
			get
			{
				return (this.Columns % 4) == 0;
			}
		}

		/// <summary>
		/// Gets the size of the image in pixels.
		/// </summary>
		public int SizeInPixels
		{
			get
			{
				if (_sizeInPixels == -1)
					_sizeInPixels = this.Rows * this.Columns;

				return _sizeInPixels;
			}
		}

		/// <summary>
		/// Gets the size of the image in bytes.
		/// </summary>
		public int SizeInBytes
		{
			get
			{
				// Only calculate this once
				if (_sizeInBytes == -1)
					_sizeInBytes = this.SizeInPixels * this.BitsPerPixel / 8;

				return _sizeInBytes;
			}
		}

		/// <summary>
		/// Gets the number of columns when the image has been aligned on a 4-byte boundary.
		/// </summary>
		public int DoubleWordAlignedColumns
		{
			get
			{
				// Only calculate this once
				if (_doubleWordAlignedColumns == -1)
				{
					// If we're not on a 4-byte boundary, round up to the next multiple of 4
					// using integer division
					if (this.Columns % 4 != 0)
						_doubleWordAlignedColumns = this.Columns / 4 * 4 + 4;
					else
						_doubleWordAlignedColumns = this.Columns;
				}

				return _doubleWordAlignedColumns;
			}
		}

		/// <summary>
		/// Gets the current interpolation method.
		/// </summary>
		public virtual InterpolationMode InterpolationMode
		{
			get { return _interpolationMode; }
		}

		/// <summary>
		/// Gets an object that encapsulates the pixel data.
		/// </summary>
		public PixelData PixelData
		{
			get
			{
				if (_pixelDataWrapper == null)
					_pixelDataWrapper = CreatePixelDataWrapper();

				return _pixelDataWrapper; 
			}
		}

		/// <summary>
		/// Gets the tightest bounding box that encloses the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public override RectangleF BoundingBox
		{
			get
			{
				// TODO fix this algorithm to work when the ImageGraphic is not also the provider of the "Source" coordinate space
				RectangleF rect = new RectangleF(PointF.Empty, new SizeF(this.Columns, this.Rows));
				if (this.CoordinateSystem == CoordinateSystem.Destination)
					return Mathematics.RectangleUtilities.ConvertToPositiveRectangle(this.SpatialTransform.ConvertToDestination(rect));
				return rect;
			}
		}

		#endregion

		#region Protected properties/methods

		/// <summary>
		/// Gets the raw raw pixel data, if passed in during construction.
		/// </summary>
		/// <value>The raw pixel data, or <b>null</b> if it was not
		/// passed in during construction</value>
		protected byte[] PixelDataRaw
		{
			get { return _pixelDataRaw; }
		}

		/// <summary>
		/// Gets the pixel data delegate, if passed in during construction.
		/// </summary>
		/// <value>The pixel data delegate, or <b>null</b> if it was not
		/// passed in during construction.</value>
		protected PixelDataGetter PixelDataGetter
		{
			get { return _pixelDataGetter; }
		}

		/// <summary>
		/// Creates an object that encapsulates the pixel data.
		/// </summary>
		/// <returns></returns>
		protected abstract PixelData CreatePixelDataWrapper();

		#endregion

		#region Public methods

		/// <summary>
		/// Performs a hit test on the <see cref="ImageGraphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns><b>True</b> if <paramref name="point"/> is within the boundaries
		/// of the image, <b>false</b> otherwise.</returns>
		public override bool HitTest(Point point)
		{
			// TODO fix this algorithm to work when the ImageGraphic is not also the provider of the "Source" coordinate space
			PointF srcPoint = this.SpatialTransform.ConvertToSource(point);

			if (srcPoint.X >= 0.0 &&
				srcPoint.X < _columns &&
				srcPoint.Y >= 0.0 &&
				srcPoint.Y < _rows)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the point on the <see cref="ImageGraphic"/> closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// </remarks>
		public override PointF GetClosestPoint(PointF point)
		{
			RectangleF rect = this.BoundingBox;
			if (rect.Contains(point))
				return point;
			return RectanglePrimitive.GetClosestPoint(point, rect);
		}

		/// <summary>
		/// Moves the <see cref="ImageGraphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public override void Move(SizeF delta)
		{
			//this.SpatialTransform.
		}

		#endregion
	}
}
