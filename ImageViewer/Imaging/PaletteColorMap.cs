using System.Drawing;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class PaletteColorMap : ColorMap
	{
		private int _size;
		private int _bitsPerLutEntry;
		private byte[] _redLut;
		private byte[] _greenLut;
		private byte[] _blueLut;

		public PaletteColorMap(
			int size,
			int firstMappedPixel,
			int bitsPerLutEntry,
			byte[] redLut,
			byte[] greenLut,
			byte[] blueLut)
		{
			_size = size;
			this.MinInputValue = firstMappedPixel;
			this.MaxInputValue = firstMappedPixel + size - 1;
			_bitsPerLutEntry = bitsPerLutEntry;
			_redLut = redLut;
			_greenLut = greenLut;
			_blueLut = blueLut;
		}

		protected override void Create()
		{
			Color color;

			if (_bitsPerLutEntry == 8)
			{
				// Account for case where an 8-bit entry is encoded in a 16 bits allocated
				// i.e., 8 bits of padding per entry
				if (_redLut.Length == 2 * _size)
				{
					for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
					{
						// Get the low byte of the 16-bit entry
						int offset = 2 * i;
						int r = _redLut[offset];
						int g = _greenLut[offset];
						int b = _blueLut[offset];
						color = Color.FromArgb(255, r, g, b);
						this[i] = color.ToArgb();
					}
				}
				// The regular 8-bit case
				else
				{
					for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
					{
						color = Color.FromArgb(255, _redLut[i], _greenLut[i], _blueLut[i]);
						this[i] = color.ToArgb();
					}
				}
			}
			// 16 bit entries
			else
			{
				for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
				{
					// Just get the high byte, since we'd have to right shift the
					// 16-bit value by 8 bits to scale it to an 8 bit value anyway.
					int offset = 2 * i + 1;
					int r = _redLut[offset];
					int g = _greenLut[offset];
					int b = _blueLut[offset];
					color = Color.FromArgb(255, r, g, b);
					this[i] = color.ToArgb();
				}
			}
		}

		public override string GetDescription()
		{
			return SR.DescriptionPaletteColorMap;
		}
	}
}
