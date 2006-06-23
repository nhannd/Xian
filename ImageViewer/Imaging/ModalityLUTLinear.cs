using System;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Imaging
{
	/// <summary>
	/// Summary description for ModalityLUTLinear.
	/// </summary>
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

			if (rescaleSlope == 0)
				_rescaleSlope = 1;
			else
				_rescaleSlope = rescaleSlope;

			_rescaleIntercept = rescaleIntercept;

			SetInputRange(bitsStored, pixelRepresentation);

			_minOutputValue = this[_minInputValue];
			_maxOutputValue = this[_maxInputValue];
		}

		public double RescaleSlope
		{
			get { return _rescaleSlope; }
		}

		public double RescaleIntercept
		{
			get { return _rescaleIntercept; }
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
	}
}