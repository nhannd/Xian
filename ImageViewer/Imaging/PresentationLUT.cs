using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a Presentation LUT.
	/// </summary>
	/// <remarks>
	/// The <see cref="PresentationLUT"/> is always the last LUT in the
	/// <see cref="LUTCollection"/>.  The values in the LUT represent
	/// ARGB values that are used by the <see cref="IRenderer"/>
	/// to display the image.
	/// </remarks>
	public class PresentationLUT : ComposableLUT
	{
		private bool _invert = false;
		private bool _lutCreated = false;
		private int _minPlusMax;

		/// <summary>
		/// Initializes a new instance of <see cref="PresentationLUT"/>
		/// with the specified mininum/maximum input values and
		/// photometric interpretation.
		/// </summary>
		/// <param name="minInputValue"></param>
		/// <param name="maxInputValue"></param>
		/// <param name="photometricInterpretation"></param>
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

		/// <summary>
		/// Not applicable.
		/// </summary>
		public override int MinOutputValue
		{
			get
			{
				throw new InvalidOperationException("A Presentation LUT cannot have a minimum output value. ");
			}
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		public override int MaxOutputValue
		{
			get
			{
				throw new InvalidOperationException("A Presentation LUT cannot have a maximum output value. ");
			}
		}

		/// <summary>
		/// Gets or sets a valud indicating whether the LUT is inverted.
		/// </summary>
		public bool Invert
		{
			get { return _invert; }
			set { _invert = value; }
		}

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns>A 32-bit ARGB value.</returns>
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
			int maxGrayLevel = this.Length - 1;

			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				float scale = (float)j / (float)maxGrayLevel;
				j++;

				int value = (int)(byte.MaxValue * scale);
				color = Color.FromArgb(255, value, value, value);
				this[i] = color.ToArgb();
			}
		}
	}
}
