using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a Modality LUT.
	/// </summary>
	public class ModalityLUTLinear : CalculatedLUT
	{
		private double _rescaleSlope;
		private double _rescaleIntercept;

		/// <summary>
		/// Initializes a new instance of ModalityLUTLinear with
		/// the specified parameters.
		/// </summary>
		/// <param name="bitsStored"></param>
		/// <param name="pixelRepresentation"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
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

		/// <summary>
		/// Gets the rescale slope.
		/// </summary>
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

		/// <summary>
		/// Gets the rescale intercept.
		/// </summary>
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

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
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