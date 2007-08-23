using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a Modality LUT.
	/// </summary>
	internal sealed class ModalityLutLinear : DataLut, IModalityLut
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
		/// <param name="pixelRepresentation"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		public ModalityLutLinear(
			int bitsStored,
			bool isSigned, 
			double rescaleSlope,
			double rescaleIntercept)
			: base(	GetMinimumInput(bitsStored, isSigned), 
					GetMaximumInput(bitsStored, isSigned), 
					GetMinimumOutput(bitsStored, isSigned, rescaleSlope, rescaleIntercept),
					GetMaximumOutput(bitsStored, isSigned, rescaleSlope, rescaleIntercept))
		{
			DicomValidator.ValidateBitsStored(bitsStored);

			_bitsStored = bitsStored;
			_isSigned = isSigned;
			this.RescaleSlope = rescaleSlope;
			this.RescaleIntercept = rescaleIntercept;
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

		public override string GetKey()
		{
			return GetKey(this.BitsStored, this.IsSigned, this.RescaleSlope, this.RescaleIntercept);
		}

		protected override void  CreateLut()
		{
			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				base[i] = (int) (this.RescaleSlope * i + this.RescaleIntercept);
			}
		}

		private static int GetMinimumInput(int bitsStored, bool isSigned)
		{
			if (isSigned)
				return -(1 << (bitsStored - 1));
			else
				return 0;
		}

		private static int GetMaximumInput(int bitsStored, bool isSigned)
		{
			if (isSigned)
				return (1 << (bitsStored - 1)) - 1;
			else
				return (1 << bitsStored) - 1;
		}

		private static int GetMinimumOutput(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
		{
			int minimumInputValue = GetMinimumInput(bitsStored, isSigned);
			int maximumInputValue = GetMaximumInput(bitsStored, isSigned);

			return (int)Math.Min(rescaleSlope * minimumInputValue + rescaleIntercept, rescaleSlope * maximumInputValue + rescaleIntercept);
		}

		private static int GetMaximumOutput(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
		{
			int minimumInputValue = GetMinimumInput(bitsStored, isSigned);
			int maximumInputValue = GetMaximumInput(bitsStored, isSigned);

			return (int)Math.Max(rescaleSlope * minimumInputValue + rescaleIntercept, rescaleSlope * maximumInputValue + rescaleIntercept);
		}

		internal static string GetKey(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
		{
			return String.Format("{0}_{1}_{2}_{3:F2}",
				bitsStored,
				isSigned.ToString(),
				rescaleSlope,
				rescaleIntercept);
		}

		public override LutCreationParameters GetCreationParametersMemento()
		{
			throw new Exception("Modality Luts do not currently support undo/redo operations.");
		}

		public override bool TrySetCreationParametersMemento(LutCreationParameters creationParameters)
		{
			throw new Exception("Modality Luts do not currently support undo/redo operations.");
		}
	}
}