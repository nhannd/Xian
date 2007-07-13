using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An image where pixel values are ARGB.
	/// </summary>
	/// <remarks>
	/// A typical usage of this class is overlaying
	/// colour regions on a grayscale image to highlight areas of interest.  
	/// Note that you can control not just the colour, but also the 
	/// opacity (i.e. alpha) of each pixel.
	/// </remarks>
	public class ColorImageGraphic : ImageGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// Creates an empty colour image of a specific size.
		/// By default, all pixels are set to ARGB=(0,0,0,0) (i.e.,
		/// transparent). 
		/// </remarks>
		public ColorImageGraphic(int rows, int columns)
			: base(rows, columns, 32)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelData">The pixel data in ARGB format.</param>
		/// <remarks>
		/// Creates a colour image using existing pixel data.
		/// </remarks>
		public ColorImageGraphic(int rows, int columns, byte[] pixelData)
			: base(rows, columns, 32, pixelData)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// Creates a grayscale image using existing pixel data but does so
		/// without ever storing a reference to the pixel data. This is necessary
		/// to ensure that pixel data can be properly garbage collected in
		/// any future memory management schemes.
		/// </remarks>
		public ColorImageGraphic(int rows, int columns, PixelDataGetter pixelDataGetter)
			: base(rows, columns, 32, pixelDataGetter)
		{
		}

		/// <summary>
		/// Gets an object that encapsulates the pixel data.
		/// </summary>
		public new ColorPixelData PixelData
		{
			get
			{
				return base.PixelData as ColorPixelData;
			}
		}

		/// <summary>
		/// Creates an object that encapsulates the pixel data.
		/// </summary>
		/// <returns></returns>
		protected override PixelData CreatePixelDataWrapper()
		{
			if (this.PixelDataRaw != null)
				return new ColorPixelData(this.Rows, this.Columns, this.PixelDataRaw);
			else
				return new ColorPixelData(this.Rows, this.Columns, this.PixelDataGetter);
		}
	}
}
