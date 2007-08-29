using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class VoiLutLinearBase : Lut
	{
		private int _minInputValue;
		private int _maxInputValue;
		private double _windowRegionStart;
		private double _windowRegionEnd;
		private bool _recalculate;

		internal VoiLutLinearBase()
		{
			_recalculate = true;
			_minInputValue = int.MinValue;
			_maxInputValue = int.MaxValue;
		}

		protected abstract double GetWindowWidth();
		protected abstract double GetWindowCenter();

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public sealed override int this[int index]
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

		public sealed override int MinInputValue
		{
			get { return _minInputValue; }
			set
			{
				if (_minInputValue == value)
					return;

				_minInputValue = value;
				Recalculate();
			}
		}

		public sealed override int MaxInputValue
		{
			get { return _maxInputValue; }
			set
			{
				if (_maxInputValue == value)
					return;

				_maxInputValue = value;
				Recalculate();
			}
		}

		public sealed override int MinOutputValue
		{
			get { return _minInputValue; }
			protected set {throw new InvalidOperationException("The minimum output value is not settable.");}
		}

		public sealed override int MaxOutputValue
		{
			get { return _maxInputValue; }
			protected set { throw new InvalidOperationException("The maximum output value is not settable."); }
		}

		public sealed override string GetKey()
		{
			return String.Format("{0}-{1}-{2}-{3}",
				this.MinInputValue,
				this.MinInputValue,
				this.GetWindowWidth(),
				this.GetWindowCenter());
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
