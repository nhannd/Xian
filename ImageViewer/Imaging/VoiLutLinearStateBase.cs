using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class VoiLutLinearStateBase
		: CalculatedLUT, IVOILUTLinear
	{
		private double _windowRegionStart;
		private double _windowRegionEnd;
		private int _outputRange;
		private bool _recalculate;

		public VoiLutLinearStateBase(int minInputValue, int maxInputValue)
		{
			if (minInputValue >= maxInputValue)
				throw new ArgumentException(SR.ExceptionLUTMinGreaterThanEqualToMax);

			this.MinInputValue = minInputValue;
			this.MaxInputValue = maxInputValue;
			this.MinOutputValue = minInputValue;
			this.MaxOutputValue = maxInputValue;
			_outputRange = this.MaxOutputValue - this.MinOutputValue;

			_recalculate = true;
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

				if (_recalculate)
				{
					Calculate();
					_recalculate = false;
				}

				if (index <= _windowRegionStart)
				{
					return this.MinOutputValue;
				}
				else if (index > _windowRegionEnd)
				{
					return this.MaxOutputValue;
				}
				else
				{
					double scale = ((index - (this.WindowCenter - 0.5)) / (this.WindowWidth - 1)) + 0.5;
					return (int)((scale * _outputRange) + this.MinOutputValue);
				}
			}
			set
			{
				throw new InvalidOperationException("Cannot set elements in a calculated LUT");
			}
		}

		public override string GetKey()
		{
			return String.Format("{0}_{1}_{2:F2}_{3:F2}",
				this.MinInputValue,
				this.MaxInputValue,
				this.WindowWidth,
				this.WindowCenter);
		}

		protected override void NotifyLUTChanged()
		{
			base.NotifyLUTChanged();
		}

		protected void Recalculate()
		{
			base.NotifyLUTChanged();
			_recalculate = true;
		}

		private void Calculate()
		{
			double halfWindow = (this.WindowWidth - 1) / 2;
			_windowRegionStart = this.WindowCenter - 0.5 - halfWindow;
			_windowRegionEnd = this.WindowCenter - 0.5 + halfWindow;
		}

		#region IVOILUTLinear Members

		public abstract double WindowWidth { get; set; }
		public abstract double WindowCenter { get; set; }

		#endregion
	}
}
