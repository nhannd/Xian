#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Represents a static region of interest for the purposes of computing statistics on the contained pixels.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Roi"/> objects are static definitions of a region of interest on a particular image. Its
	/// shape definition and the underlying pixel values are considered fixed upon construction, and hence its
	/// various properties and statistics are non-changing.
	/// </para>
	/// <para>
	/// New instances of a <see cref="Roi"/> should be constructed everytime the definition of the region of
	/// interest or the underlying image pixel data has changed. The <see cref="IGraphic.GetRoi"/>
	/// method allows client code to quickly construct a new instance of a <see cref="Roi"/> based on the current
	/// definition of the graphic and the image it currently belongs to.
	/// </para>
	/// </remarks>
	public abstract class Roi
	{
		private readonly int _imageRows;
		private readonly int _imageColumns;
		private readonly string _modality;
		private readonly PixelData _pixelData;
		private readonly PixelAspectRatio _pixelAspectRatio;
		private readonly PixelSpacing _normalizedPixelSpacing;
		private readonly IModalityLut _modalityLut;

		private RectangleF _boundingBox;

		/// <summary>
		/// Constructs a new region of interest, specifying an <see cref="IPresentationImage"/> as the source of the pixel data.
		/// </summary>
		/// <param name="presentationImage">The image containing the source pixel data.</param>
		protected Roi(IPresentationImage presentationImage)
		{
            PresentationImage = presentationImage;
			IImageGraphicProvider provider = presentationImage as IImageGraphicProvider;
			if (provider == null)
				return;

			_imageRows = provider.ImageGraphic.Rows;
			_imageColumns = provider.ImageGraphic.Columns;

			_pixelData = provider.ImageGraphic.PixelData;
			if (presentationImage is IModalityLutProvider)
				_modalityLut = ((IModalityLutProvider) presentationImage).ModalityLut;

			if (presentationImage is IImageSopProvider)
			{
				Frame frame = ((IImageSopProvider) presentationImage).Frame;
				_normalizedPixelSpacing = frame.NormalizedPixelSpacing;
				_pixelAspectRatio = frame.PixelAspectRatio;
				_modality = frame.ParentImageSop.Modality;
			}
			else
			{
				_normalizedPixelSpacing = new PixelSpacing(0, 0);
				_pixelAspectRatio = new PixelAspectRatio(0, 0);
			}
		}

        ///<summary>
        /// Gets the presentation image
        ///</summary>
        public IPresentationImage PresentationImage
        {
            get;
            private set;
        }

		/// <summary>
		/// Gets the number of rows in the entire image.
		/// </summary>
		public int ImageRows
		{
			get { return _imageRows; }
		}

		/// <summary>
		/// Gets the number of columns in the entire image.
		/// </summary>
		public int ImageColumns
		{
			get { return _imageColumns; }
		}

		/// <summary>
		/// Gets the pixel dimensions of the image.
		/// </summary>
		public Size ImageSize
		{
			get { return new Size(_imageColumns, _imageRows); }
		}

		/// <summary>
		/// Gets the pixel data of the image.
		/// </summary>
		public PixelData PixelData
		{
			get { return _pixelData; }
		}

		/// <summary>
		/// Gets the pixel aspect ratio of the image.
		/// </summary>
		public PixelAspectRatio PixelAspectRatio
		{
			get { return _pixelAspectRatio; }
		}

		/// <summary>
		/// Gets the normalized pixel spacing of the image.
		/// </summary>
		public PixelSpacing NormalizedPixelSpacing
		{
			get { return _normalizedPixelSpacing; }
		}

		/// <summary>
		/// Gets the modality LUT of the image, if one exists.
		/// </summary>
		public IModalityLut ModalityLut
		{
			get { return _modalityLut; }
		}

		/// <summary>
		/// Gets the modality of the image.
		/// </summary>
		public string Modality
		{
			get { return _modality; }
		}

		/// <summary>
		/// Gets the tightest bounding box containing the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		public RectangleF BoundingBox
		{
			get
			{
				//TODO (cr Feb 2010): convert to positive rectangle as a safety precaution.
				if (_boundingBox.IsEmpty)
					_boundingBox = ComputeBounds();
				return _boundingBox;
			}
		}

		/// <summary>
		/// Called by <see cref="BoundingBox"/> to compute the tightest bounding box of the region of interest.
		/// </summary>
		/// <remarks>
		/// <para>This method is only called once and the result is cached for future accesses.</para>
		/// <para>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </para>
		/// </remarks>
		/// <returns>A rectangle defining the bounding box.</returns>
		protected abstract RectangleF ComputeBounds();

		/// <summary>
		/// Creates a copy of this <see cref="Roi"/> using the same region of interest shape but using a different image as the source pixel data.
		/// </summary>
		/// <param name="presentationImage">The image upon which to copy this region of interest.</param>
		/// <returns>A new <see cref="Roi"/> of the same type and shape as the current region of interest.</returns>
		public abstract Roi CopyTo(IPresentationImage presentationImage);

		/// <summary>
		/// Tests to see if the given point is contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <param name="point">The point to test.</param>
		/// <returns>True if the point is defined as within the region of interest; False otherwise.</returns>
		public abstract bool Contains(PointF point);

		/// <summary>
		/// Tests to see whether or not the region of interest actual contains any pixel data.
		/// </summary>
		/// <returns>True if the region of interest contains pixel data; False otherwise..</returns>
		public virtual bool ContainsPixelData
		{
			get { return IsBoundingBoxInImage(); }
		}

        

	    /// <summary>
		/// Tests to see if the given point is contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <param name="x">The X-coordinate of the point to test.</param>
		/// <param name="y">The Y-coordinate of the point to test.</param>
		/// <returns>True if the point is defined as within the region of interest; False otherwise.</returns>
		public bool Contains(float x, float y)
		{
			return this.Contains(new PointF(x, y));
		}

		/// <summary>
		/// Tests to see if the given point is contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <param name="x">The X-coordinate of the point to test.</param>
		/// <param name="y">The Y-coordinate of the point to test.</param>
		/// <returns>True if the point is defined as within the region of interest; False otherwise.</returns>
		public bool Contains(int x, int y)
		{
			return this.Contains(new PointF(x, y));
		}

		/// <summary>
		/// Gets an enumeration of the coordinates of points contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <returns>An enumeration of points.</returns>
		public IEnumerable<PointF> GetPixelCoordinates()
		{
			//TODO (CR Sept 2010): should be RoundInflate, not Ceiling.
			Rectangle bounds = Rectangle.Ceiling(this.BoundingBox);
			bounds.Intersect(new Rectangle(Point.Empty, this.ImageSize));
			for (int x = bounds.Left; x < bounds.Right; x++)
			{
				for (int y = bounds.Top; y < bounds.Bottom; y++)
				{
					PointF p = new PointF(x, y);
					if (this.Contains(p))
						yield return p;
				}
			}
		}

		/// <summary>
		/// Gets an enumeration of the raw pixel values contained within the region of interest.
		/// </summary>
		/// <returns>An enumeration of raw pixel values.</returns>
		public IEnumerable<int> GetRawPixelValues()
		{
			if (this.PixelData == null)
				yield break;

			//TODO (CR Sept 2010): should be RoundInflate, not Ceiling.
			Rectangle bounds = Rectangle.Ceiling(this.BoundingBox);
			bounds.Intersect(new Rectangle(Point.Empty, this.ImageSize));
			for (int x = bounds.Left; x < bounds.Right; x++)
			{
				for (int y = bounds.Top; y < bounds.Bottom; y++)
				{
					PointF p = new PointF(x, y);
					if (this.Contains(p))
						yield return this.PixelData.GetPixel(x, y);
				}
			}
		}

		/// <summary>
		/// Gets an enumeration of the modality LUT transformed pixel values contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// If the <see cref="ModalityLut"/> is null, then this method enumerates the same values as <see cref="GetRawPixelValues"/>.
		/// </remarks>
		/// <returns>An enumeration of modality LUT transformed pixel values.</returns>
		[Obsolete]
		// TODO CR (Oct 11): replace entirely with one that returns doubles but without the real world units - breaking existing code is the least of our worries since anyone using this old version might get wrong values
		public IEnumerable<int> GetPixelValues()
		{
			int dummy;
			foreach (var pixelValue in GetPixelValues(out dummy))
				yield return (int) Math.Round(pixelValue);
		}

		public IEnumerable<double> GetPixelValues(out int realWorldUnits)
		{
			realWorldUnits = 0;
			if (this.PixelData == null)
				return new double[0];

			List<double> values = new List<double>();
			LutFunction lut = v => v;
			if (this.ModalityLut != null)
				lut = v => this.ModalityLut[v];

			//TODO (CR Sept 2010): put these 2 lines into a method, since it's used numerous times.  Even if it were
			//only used once, a method name tells someone what this does instead of having to figure it out.

			//TODO (CR Sept 2010): should be RoundInflate, not Ceiling.
			Rectangle bounds = Rectangle.Ceiling(this.BoundingBox);
			bounds.Intersect(new Rectangle(Point.Empty, this.ImageSize));
			for (int x = bounds.Left; x < bounds.Right; x++)
			{
				for (int y = bounds.Top; y < bounds.Bottom; y++)
				{
					PointF p = new PointF(x, y);
					if (this.Contains(p))
						values.Add(lut(this.PixelData.GetPixel(x, y)));
				}
			}
			return values;
		}

		/// <summary>
		/// Checks if operations in the given <paramref name="units"/> are possible with the <paramref name="pixelSpacing"/> information available.
		/// </summary>
		/// <returns>True if such operations are valid; False if no such operation is possible.</returns>
		protected static bool ValidateUnits(Units units, PixelSpacing pixelSpacing)
		{
			return (units == Units.Pixels || !pixelSpacing.IsNull);
		}

		/// <summary>
		/// Converts an area in pixels into the given units given some particular pixel spacing.
		/// </summary>
		/// <param name="area">The area of pixels to be converted.</param>
		/// <param name="units">The units into which the area should be converted.</param>
		/// <param name="pixelSpacing">The pixel spacing information available.</param>
		/// <returns>The equivalent area in the units of <paramref name="units"/>.</returns>
		/// <exception cref="ArgumentException">Thrown if <paramref name="units"/> is a physical unit of measurement and <paramref name="pixelSpacing"/> is not calibrated.</exception>
		protected static double ConvertFromSquarePixels(double area, Units units, PixelSpacing pixelSpacing)
		{
			if (!ValidateUnits(units, pixelSpacing))
				throw new ArgumentException("Pixel spacing must be calibrated in order to compute physical units.", "units");

			double factor = 1;

			switch (units)
			{
				case Units.Pixels:
					factor = 1;
					break;
				case Units.Centimeters:
					factor = pixelSpacing.Column*pixelSpacing.Row/100;
					break;
				case Units.Millimeters:
				default:
					factor = pixelSpacing.Column*pixelSpacing.Row;
					break;
			}

			return area*factor;
		}

		/// <summary>
		/// Checks whether or not the region of interest's bounding box intersects the image.
		/// </summary>
		/// <returns>True if the bounding box intersects the image; False otherwise.</returns>
		private bool IsBoundingBoxInImage()
		{
			RectangleF boundingBox = RectangleUtilities.ConvertToPositiveRectangle(this.BoundingBox);

			if (boundingBox.Width == 0 || boundingBox.Height == 0)
				return false;

			if (boundingBox.Left < 0 ||
				boundingBox.Top < 0 ||
				boundingBox.Right > (this.ImageColumns - 1) ||
				boundingBox.Bottom > (this.ImageRows - 1))
				return false;

			return true;
		}

		private delegate double LutFunction(int v);
	}
}