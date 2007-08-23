using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class VoiLutLinearBase : Lut
	{
		private int _minInputValue;
		private int _maxInputValue;
		private double _windowRegionStart;
		private double _windowRegionEnd;
		private bool _recalculate;

		internal VoiLutLinearBase(int minInputValue, int maxInputValue)
		{
			if (minInputValue >= maxInputValue)
				throw new ArgumentException(SR.ExceptionLUTMinGreaterThanEqualToMax);

			_minInputValue = minInputValue;
			_maxInputValue = maxInputValue;

			_recalculate = true;
		}

		private VoiLutLinearBase()
		{
		}

		protected abstract double GetWindowWidth();
		protected abstract double GetWindowCenter();

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override int this[int index]
		{
			get
			{
				if (_recalculate)
				{
					Calculate();
					_recalculate = false;
				}

				if (index < _windowRegionStart)
				{
					return this.MinOutputValue;
				}
				else if (index > _windowRegionEnd)
				{
					return this.MaxOutputValue;
				}
				else
				{
					double scale = ((index - (this.GetWindowCenter() - 0.5)) / (this.GetWindowWidth() - 1)) + 0.5;
					return (int)((scale * (this.MaxOutputValue - this.MinOutputValue)) + this.MinOutputValue);
				}
			}
			protected set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int MinInputValue
		{
			get { return _minInputValue; }
		}

		public override int MaxInputValue
		{
			get { return _maxInputValue; }
		}

		public override int MinOutputValue
		{
			get { return _minInputValue; }
		}

		public override int MaxOutputValue
		{
			get { return _maxInputValue; }
		}

		protected void Recalculate()
		{
			base.OnLutChanged();
			_recalculate = true;
		}

		private void Calculate()
		{
			double halfWindow = (this.GetWindowWidth() - 1) / 2;
			_windowRegionStart = this.GetWindowCenter() - 0.5 - halfWindow;
			_windowRegionEnd = this.GetWindowCenter() - 0.5 + halfWindow;
		}
	}
}
