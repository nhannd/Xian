using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class ColorImageGraphic : ImageGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="IndexedImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelData"></param>
		public ColorImageGraphic(
			int rows,
			int columns,
			byte[] pixelData)
			: base(
				rows,
				columns,
				32,
				pixelData)
		{
		}

		protected override PixelData CreatePixelDataWrapper()
		{
			return new ColorPixelData(
						this.Rows,
						this.Columns,
						this.PixelDataRaw);
		}
	}
}
