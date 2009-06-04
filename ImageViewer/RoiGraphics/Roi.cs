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

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	//TODO (CR May09):LinearUnits property, just get Length property

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
	/// interest or the underlying image pixel data has changed. The <see cref="IGraphic.CreateRoi"/>
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
		private readonly IComposableLut _modalityLut;

		private RectangleF _boundingBox;

		/// <summary>
		/// Constructs a new region of interest, specifying an <see cref="IPresentationImage"/> as the source of the pixel data.
		/// </summary>
		/// <param name="presentationImage">The image containing the source pixel data.</param>
		protected Roi(IPresentationImage presentationImage)
		{
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
		public IComposableLut ModalityLut
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

		//TODO (CR May09): GetPixelCoordinates

		/// <summary>
		/// Enumerates the coordinates of points contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// Regions of interest have no notion of coordinate system. All coordinates are inherently
		/// given relative to the image pixel space (i.e. <see cref="CoordinateSystem.Source"/>.)
		/// </remarks>
		/// <returns>An enumeration of points.</returns>
		public IEnumerable<PointF> EnumeratePoints()
		{
			Rectangle bounds = Rectangle.Round(this.BoundingBox);
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

		//TODO (CR May09): GetRawPixelValues

		/// <summary>
		/// Enumerates the raw pixel values contained within the region of interest.
		/// </summary>
		/// <returns>An enumeration of raw pixel values.</returns>
		public IEnumerable<int> EnumerateRawPixels()
		{
			if (this.PixelData == null)
				yield break;

			Rectangle bounds = Rectangle.Round(this.BoundingBox);
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
		/// Enumerates the modality LUT transformed pixel values contained within the region of interest.
		/// </summary>
		/// <remarks>
		/// If the <see cref="ModalityLut"/> is null, then this method enumerates the same values as <see cref="EnumerateRawPixels"/>.
		/// </remarks>
		/// <returns>An enumeration of modality LUT transformed pixel values.</returns>
		public IEnumerable<int> EnumeratePixels()
		{
			if (this.PixelData == null)
				yield break;

			ILut lut = this.ModalityLut;
			if (lut == null)
				lut = new IdentityLut();

			Rectangle bounds = Rectangle.Round(this.BoundingBox);
			for (int x = bounds.Left; x < bounds.Right; x++)
			{
				for (int y = bounds.Top; y < bounds.Bottom; y++)
				{
					PointF p = new PointF(x, y);
					if (this.Contains(p))
						yield return lut[this.PixelData.GetPixel(x, y)];
				}
			}
		}

		private class IdentityLut : ILut
		{
			public int this[int index]
			{
				get { return index; }
			}

			public int MinOutputValue
			{
				get { return int.MinValue; }
			}

			public int MaxOutputValue
			{
				get { return int.MaxValue; }
			}

			public int MinInputValue
			{
				get { return int.MinValue; }
			}

			public int MaxInputValue
			{
				get { return int.MaxValue; }
			}
		}
	}
}