using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;


namespace ClearCanvas.ImageViewer.Imaging
{
	internal class WindowLevelMemento : IMemento
	{
		private double _windowWidth;
		private double _windowCenter;

		public WindowLevelMemento(double windowWidth, double windowCenter)
		{
			_windowWidth = windowWidth;
			_windowCenter = windowCenter;	
		}

		public double WindowWidth
		{
			get { return _windowWidth; }
			set { _windowWidth = value; }
		}

		public double WindowCenter
		{
			get { return _windowCenter; }
			set { _windowCenter = value; }
		}
	}

	public class VoiLutLinear
		: CalculatedLut, IVoiLutLinear
	{
		private double _windowWidth = 1;
		private double _windowCenter = 0;
		private double _windowRegionStart;
		private double _windowRegionEnd;
		private int _outputRange;
		private bool _recalculate;

		public VoiLutLinear(int minInputValue, int maxInputValue)
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

		/// <summary>
		/// Gets or sets the window width.
		/// </summary>
		public double WindowWidth
		{
			get { return _windowWidth; }
			set
			{
				if (value == _windowWidth)
					return;

				if (value < 1)
					value = 1;

				_windowWidth = value;
				Recalculate();
			}
		}

		/// <summary>
		/// Gets or sets the window center.
		/// </summary>
		public double WindowCenter
		{
			get { return _windowCenter; }
			set
			{
				if (value == _windowCenter)
					return;

				_windowCenter = value;
				Recalculate();
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


		#region IMemorable Members

		public IMemento CreateMemento()
		{
			WindowLevelMemento memento = new WindowLevelMemento(
				this.WindowWidth, 
				this.WindowCenter);

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

		#region IVoiLut Members

		public string Name
		{
			get { return "Dynamic Window/Level"; }
		}

		#endregion
	}
}
