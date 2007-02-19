using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class PresentationLUT : ComposableLUT
	{
		private bool _invert = false;
		private bool _lutCreated = false;
		private int _minPlusMax;

		public PresentationLUT(
			int minInputValue, 
			int maxInputValue,
			PhotometricInterpretation photometricInterpretation)
			: base(minInputValue, maxInputValue)
		{
			if (photometricInterpretation == PhotometricInterpretation.Monochrome1)
				_invert = true;

			_minPlusMax = this.MinInputValue + this.MaxInputValue;
		}

		public bool Invert
		{
			get { return _invert; }
			set { _invert = value; }
		}

		public override int this[int index]
		{
			get
			{
				if (!_lutCreated)
				{
					CreateGrayscaleLUT();
					_lutCreated = true;
				}

				if (_invert)
					return base[_minPlusMax - index];
				else
					return base[index];
			}
		}

		private void CreateColorLUT()
		{
			Color color1 = Color.Red;
			Color color2 = Color.Yellow;
			Color color;

			int start = -5000;
			int end = 10000;
			int range = end - start;
			int j = 0;

			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				if (i < start)
				{
					color = Color.Black;
				}
				else if (i > end)
				{
					color = Color.White;
				}
				else
				{
					double scale = (double)j / (double)range;
					int r = (int)(scale * Math.Abs(color2.R - color1.R) + color1.R);
					int g = (int)(scale * Math.Abs(color2.G - color1.G) + color1.G);
					int b = (int)(scale * Math.Abs(color2.B - color1.B) + color1.B);

					color = Color.FromArgb(255, r, g, b);
					j++;
				}

				this[i] = color.ToArgb();
			}

		}

		private void CreateGrayscaleLUT()
		{
			Color color;

			int j = 0;

			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				float scale = (float)j / (float)this.NumEntries;
				j++;

				int value = (int)(byte.MaxValue * scale);
				color = Color.FromArgb(255, value, value, value);
				this[i] = color.ToArgb();
			}
		}
	}
}
