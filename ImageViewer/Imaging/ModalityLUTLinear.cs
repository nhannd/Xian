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
				Platform.CheckIndexRange(index, this.MinInputValue, this.MaxInputValue, this);

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
				this.MinInputValue = 0;
				this.MaxInputValue = (1 << bitsStored) - 1;
			}
			else
			{
				this.MinInputValue = -(1 << (bitsStored - 1));
				this.MaxInputValue =  (1 << (bitsStored - 1)) - 1;
			}
		}

		private void SetOutputRange()
		{
			this.MinOutputValue = this[this.MinInputValue];
			this.MaxOutputValue = this[this.MaxInputValue];
		}
	}
}