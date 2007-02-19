using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class ModalityLUTLinear : CalculatedGrayscaleLUT
	{
		private double _rescaleSlope;
		private double _rescaleIntercept;

		public ModalityLUTLinear(
			int bitsStored,
			int pixelRepresentation, 
			double rescaleSlope,
			double rescaleIntercept)
		{
			ImageValidator.ValidateBitsStored(bitsStored);
			ImageValidator.ValidatePixelRepresentation(pixelRepresentation);

			this.RescaleSlope = rescaleSlope;
			this.RescaleIntercept = rescaleIntercept;

			SetInputRange(bitsStored, pixelRepresentation);
			SetOutputRange();
		}

		public double RescaleSlope
		{
			get { return _rescaleSlope; }
			private set
			{
				if (value == 0 || double.IsNaN(value))
					_rescaleSlope = 1;
				else
					_rescaleSlope = value;
			}
		}

		public double RescaleIntercept
		{
			get { return _rescaleIntercept; }
			private set
			{
				if (double.IsNaN(value))
					_rescaleIntercept = 0;
				else
					_rescaleIntercept = value;
			}
		}

		public override int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, _minInputValue, _maxInputValue, this);

				return (int) (index * _rescaleSlope + _rescaleIntercept);
			}
			set
			{
				throw new InvalidOperationException("Cannot set elements in a calculated LUT");
			}
		}

		private void SetInputRange(int bitsStored, int pixelRepresentation)
		{
			// Determine input value range
			if (pixelRepresentation == 0)
			{
				_minInputValue = 0;
				_maxInputValue = (1 << bitsStored) - 1;
			}
			else
			{
				_minInputValue = -(1 << (bitsStored - 1));
				_maxInputValue =  (1 << (bitsStored - 1)) - 1;
			}
		}

		private void SetOutputRange()
		{
			_minOutputValue = this[_minInputValue];
			_maxOutputValue = this[_maxInputValue];
		}
	}
}