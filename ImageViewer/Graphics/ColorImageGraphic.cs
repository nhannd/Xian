using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An image
	/// </summary>
	public class ColorImageGraphic : ImageGraphic
	{
		public ColorImageGraphic(int rows, int columns)
			: base(rows, columns, 32)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelData"></param>
		public ColorImageGraphic(int rows, int columns, byte[] pixelData)
			: base(rows, columns, 32, pixelData)
		{
		}

		public ColorImageGraphic(int rows, int columns, PixelDataGetter pixelDataGetter)
			: base(rows, columns, 32, pixelDataGetter)
		{
		}

		public new ColorPixelData PixelData
		{
			get
			{
				return base.PixelData as ColorPixelData;
			}
		}

		protected override PixelData CreatePixelDataWrapper()
		{
			if (this.PixelDataRaw != null)
				return new ColorPixelData(this.Rows, this.Columns, this.PixelDataRaw);
			else
				return new ColorPixelData(this.Rows, this.Columns, this.PixelDataGetter);
		}
	}
}
