using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing all the base implementation for Linear Voi Luts.
	/// </summary>
	/// <remarks>
	/// A simple Linear Voi Lut class (<see cref="BasicVoiLutLinear"/>) and other 
	/// abstract base classes (<see cref="CalculatedVoiLutLinear"/>, <see cref="AlgorithmCalculatedVoiLutLinear"/>)
	/// are provided that cover most, if not all, Linear Voi Lut use cases.  You should not need
	/// to derive directly from this class.
	/// </remarks>
	public abstract class VoiLutLinearBase : Lut
	{
		private int _minInputValue;
		private int _maxInputValue;
		private double _windowRegionStart;
		private double _windowRegionEnd;
		private bool _recalculate;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected VoiLutLinearBase()
		{
			_recalculate = true;
			_minInputValue = int.MinValue;
			_maxInputValue = int.MaxValue;
		}

		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		protected abstract double GetWindowWidth();

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		protected abstract double GetWindowCenter();

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		/// <param name="index">the index into the Lut</param>
		/// <returns>the value at the given index</returns>
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

		/// <summary>
		/// Gets or sets the minimum input value.  This value will be set internally by the framework.
		/// </summary>
		public sealed override int MinInputValue
		{
			get { return _minInputValue; }
			set
			{
				if (_minInputValue == value)
					return;

				_minInputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum input value.  This value will be set internally by the framework.
		/// </summary>
		public sealed override int MaxInputValue
		{
			get { return _maxInputValue; }
			set
			{
				if (_maxInputValue == value)
					return;

				_maxInputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public sealed override int MinOutputValue
		{
			get { return _minInputValue; }
			protected set {throw new InvalidOperationException("The minimum output value is not settable.");}
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public sealed override int MaxOutputValue
		{
			get { return _maxInputValue; }
			protected set { throw new InvalidOperationException("The maximum output value is not settable."); }
		}

		/// <summary>
		/// Gets a string key that identifies this particular LUT's characteristics, so that 
		/// an image's <see cref="OutputLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <B>equality</B>, since some Luts can be
		/// dependent upon the actual image to which it belongs.  The method should simply 
		/// be used to determine if a lut in the <see cref="OutputLutPool"/> is the same 
		/// as an existing one.
		/// </remarks>
		public sealed override string GetKey()
		{
			return String.Format("{0}_{1}_{2}_{3}",
				this.MinInputValue,
				this.MaxInputValue,
				this.GetWindowWidth(),
				this.GetWindowCenter());
		}

		/// <summary>
		/// Should be called by implementors when the Lut characteristics have changed.
		/// </summary>
		protected override void OnLutChanged()
		{
			_recalculate = true;
			base.OnLutChanged();
		}

		private void Calculate()
		{
			double halfWindow = (this.GetWindowWidth() - 1) / 2;
			_windowRegionStart = this.GetWindowCenter() - 0.5 - halfWindow;
			_windowRegionEnd = this.GetWindowCenter() - 0.5 + halfWindow;
		}
	}
}
