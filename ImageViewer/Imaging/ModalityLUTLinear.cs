using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a Modality LUT.
	/// </summary>
	public class ModalityLUTLinear : ComposableLUT
	{
		private bool _lutCreated = false;
		private int _bitsStored;
		private bool _isSigned;
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
			bool isSigned, 
			double rescaleSlope,
			double rescaleIntercept)
		{
			ImageValidator.ValidateBitsStored(bitsStored);

			_bitsStored = bitsStored;
			_isSigned = isSigned;
			this.RescaleSlope = rescaleSlope;
			this.RescaleIntercept = rescaleIntercept;

			SetInputRange(bitsStored, isSigned);
			SetOutputRange();
		}

		internal int BitsStored
		{
			get { return _bitsStored; }
		}

		internal bool IsSigned
		{
			get { return _isSigned; }
		}

		/// <summary>
		/// Gets the rescale slope.
		/// </summary>
		internal double RescaleSlope
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
		internal double RescaleIntercept
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
				if (!_lutCreated)
				{
					CreateLUT();
					_lutCreated = true;
				}

				return base[index];
			}
		}

		public override string GetKey()
		{
			return String.Format("{0}_{1}_{2}_{3:F2}",
				this.BitsStored,
				this.IsSigned.ToString(),
				this.RescaleSlope,
				this.RescaleIntercept);
		}

		private void CreateLUT()
		{
			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				this[i] = (int) (this.RescaleSlope * i + this.RescaleIntercept);
			}
		}

		private void SetInputRange(int bitsStored, bool isSigned)
		{
			// Determine input value range
			if (isSigned)
			{
				this.MinInputValue = -(1 << (bitsStored - 1));
				this.MaxInputValue = (1 << (bitsStored - 1)) - 1;
			}
			else
			{
				this.MinInputValue = 0;
				this.MaxInputValue = (1 << bitsStored) - 1;
			}

			this.Length = this.MaxInputValue - this.MinInputValue + 1;
		}

		private void SetOutputRange()
		{
			this.MinOutputValue = this[this.MinInputValue];
			this.MaxOutputValue = this[this.MaxInputValue];
		}
	}
}