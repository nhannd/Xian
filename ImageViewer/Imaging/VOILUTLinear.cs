using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class VOILUTLinear : 
		CalculatedGrayscaleLUT, 
		IVOILUTLinear
	{
		private double _windowWidth = 1;
		private double _windowCenter = 0;
		private double _windowRegionStart;
		private double _windowRegionEnd;
		private int _outputRange;
		private bool _recalculate;

		public VOILUTLinear(int minInputValue, int maxInputValue)
		{
			if (minInputValue >= maxInputValue)
				throw new ArgumentException(SR.ExceptionLUTMinGreaterThanEqualToMax);

			this.MinInputValue = minInputValue;
			this.MaxInputValue = maxInputValue;
			this.MinOutputValue = minInputValue;
			this.MaxOutputValue = maxInputValue;
			_outputRange = this.MaxOutputValue - this.MinOutputValue;
		}

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
					double scale = ((index - (_windowCenter - 0.5)) / (_windowWidth - 1)) + 0.5;
					return (int)((scale * _outputRange) + this.MinOutputValue);
				}
			}
			set
			{
				throw new InvalidOperationException("Cannot set elements in a calculated LUT");
			}
		}

		#region IVOILUTLinear Members

		public double WindowWidth
		{
			get { return _windowWidth; }
			set
			{
				if (value < 1)
					_windowWidth = 1;
				else
					_windowWidth = value;

				_recalculate = true;
			}
		}

		public double WindowCenter
		{
			get { return _windowCenter; }
			set
			{
				_windowCenter = value;
				_recalculate = true;
			}
		}

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			WindowLevelMemento memento = new WindowLevelMemento();

			memento.WindowWidth = this.WindowWidth;
			memento.WindowCenter = this.WindowCenter;

			return memento;
		}

		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			WindowLevelMemento windowLevelMemento = memento as WindowLevelMemento;
			Platform.CheckForInvalidCast(windowLevelMemento, "memento", "WindowLevelMemento");

			this.WindowWidth = windowLevelMemento.WindowWidth;
			this.WindowCenter = windowLevelMemento.WindowCenter;
		}

		#endregion

		#endregion

		private void Calculate()
		{
			double halfWindow = (_windowWidth - 1)  / 2;
			_windowRegionStart = _windowCenter - 0.5 - halfWindow;
			_windowRegionEnd = _windowCenter - 0.5 + halfWindow;
		}

	}
}