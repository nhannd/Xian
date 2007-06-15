using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a linear VOI LUT.
	/// </summary>
	public sealed class VOILUTLinear :
		CalculatedLUT,
		IMemorableComposableLut, 
		IVOILUTLinear
	{
		private double _windowRegionStart;
		private double _windowRegionEnd;
		private int _outputRange;
		private bool _recalculate;
		private VoiLutLinearState _state;

		public VOILUTLinear(VoiLutLinearState state, int minInputValue, int maxInputValue)
		{
			if (minInputValue >= maxInputValue)
				throw new ArgumentException(SR.ExceptionLUTMinGreaterThanEqualToMax);

			this.MinInputValue = minInputValue;
			this.MaxInputValue = maxInputValue;
			this.MinOutputValue = minInputValue;
			this.MaxOutputValue = maxInputValue;
			_outputRange = this.MaxOutputValue - this.MinOutputValue;

			this.State = state;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="VOILUTLinear"/> with
		/// the specified minimum and maximum input values.
		/// </summary>
		/// <param name="minInputValue"></param>
		/// <param name="maxInputValue"></param>
		public VOILUTLinear(int minInputValue, int maxInputValue)
			: this(new StandardVoiLutLinearState(), minInputValue, maxInputValue)
		{
		}

		public VoiLutLinearState State
		{
			get { return _state; }
			set
			{
				Platform.CheckForNullReference(value, "value");

				if (value == _state)
					return;

				_state = value;
				_state.OwnerLut = this;
				Recalculate();
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

		#region IVOILUTLinear Members

		/// <summary>
		/// Gets or sets the window width.
		/// </summary>
		public double WindowWidth
		{
			get { return _state.WindowWidth; }
			set
			{
				double oldValue = _state.WindowWidth;
				_state.WindowWidth = value;
				if (_state.WindowWidth != oldValue)
					Recalculate();
			}
		}

		/// <summary>
		/// Gets or sets the window center.
		/// </summary>
		public double WindowCenter
		{
			get { return _state.WindowCenter; }
			set
			{
				double oldValue = _state.WindowCenter;
				_state.WindowCenter = value;
				if (_state.WindowCenter != oldValue)
					Recalculate();
			}
		}

		#endregion

		public override string GetKey()
		{
			return String.Format("{0}_{1}_{2:F2}_{3:F2}",
				this.MinInputValue,
				this.MaxInputValue,
				this.WindowWidth,
				this.WindowCenter);
		}

		/// <summary>
		/// Tries to restore the state of the object.  This function will not throw an exception.
		/// </summary>
		/// <param name="memento"></param>
		public bool TrySetMemento(IMemorableComposableLutMemento memento)
		{
			VoiLutLinearState voiLutLinearMemento = memento as VoiLutLinearState;
			if (memento == null)
				return false;

			this.State = voiLutLinearMemento;
			return true;
		}

		/// <summary>
		/// Captures the state of the object.
		/// </summary>
		/// <returns></returns>
		public IMemorableComposableLutMemento CreateMemento()
		{
			return this.State.SnapshotMemento();
		}

		/// <summary>
		/// Restores the state of the object.  Will throw an exception if the memento type is unrecognized.
		/// </summary>
		/// <param name="memento"></param>
		public void SetMemento(IMemorableComposableLutMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			VoiLutLinearState state = memento as VoiLutLinearState;
			Platform.CheckForNullReference(state, "state");

			this.State = state;
		}

		protected override void NotifyLUTChanged()
		{
			base.NotifyLUTChanged();
		}

		private void Recalculate()
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
	}
}