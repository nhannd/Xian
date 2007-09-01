using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a Modality LUT.
	/// </summary>
	internal sealed class ModalityLutLinear : DataLut, IModalityLut, IEquatable<ModalityLutLinear>
	{
		private int _bitsStored;
		private bool _isSigned;
		private double _rescaleSlope;
		private double _rescaleIntercept;

		/// <summary>
		/// Initializes a new instance of ModalityLUTLinear with
		/// the specified parameters.
		/// </summary>
		/// <param name="bitsStored"></param>
		/// <param name="isSigned"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		public ModalityLutLinear(
			int bitsStored,
			bool isSigned, 
			double rescaleSlope,
			double rescaleIntercept)
			: base()
		{
			DicomValidator.ValidateBitsStored(bitsStored);

			_bitsStored = bitsStored;
			_isSigned = isSigned;
			this.RescaleSlope = rescaleSlope;
			this.RescaleIntercept = rescaleIntercept;

			Initialize();
		}

		private void Initialize()
		{
			if (this.IsSigned)
			{
				base.MinInputValue = -(1 << (this.BitsStored - 1));
				base.MaxInputValue = (1 << (this.BitsStored - 1)) - 1;
			}
			else
			{
				base.MinInputValue = 0;
				base.MaxInputValue = (1 << this.BitsStored) - 1;
			}

			int minMax1 = (int)(this.RescaleSlope*this.MinInputValue + this.RescaleIntercept);
			int minMax2 = (int) (this.RescaleSlope*this.MaxInputValue + this.RescaleIntercept);

			base.MinOutputValue = (int)Math.Min(minMax1, minMax2);
			base.MaxOutputValue = (int)Math.Max(minMax1, minMax2);
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
				if (value <= double.Epsilon || double.IsNaN(value))
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

		public override void  Create()
		{
			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				base[i] = (int) (this.RescaleSlope * i + this.RescaleIntercept);
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

		public override string GetDescription()
		{
			return String.Format("Slope: {0:F2} Int.: {1:F2}", _rescaleSlope, _rescaleIntercept);
		}

		#region IEquatable<IModalityLut> Members

		public bool Equals(IModalityLut other)
		{
			if (this == other)
				return true;

			if (other is ModalityLutLinear)
				return this.Equals(other as ModalityLutLinear);

			return false;
		}

		#endregion

		#region IEquatable<ModalityLutLinear> Members

		public bool Equals(ModalityLutLinear other)
		{
			return
				this.BitsStored == other.BitsStored && this.IsSigned == other.IsSigned && this.RescaleSlope == other.RescaleSlope &&
				this.RescaleIntercept == other.RescaleIntercept;
		}

		#endregion
	}
}