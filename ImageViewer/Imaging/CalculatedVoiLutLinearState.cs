using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class CalculatedVoiLutLinearState<T> : VoiLutLinearState
	{
		public delegate void GetWindowCenterValuesDelegate<T>(T stateData, out double windowWidth, out double windowCenter);

		private T _stateData;
		private GetWindowCenterValuesDelegate<T> _getWindowCenterValuesDelegate;

		public CalculatedVoiLutLinearState(T stateData, GetWindowCenterValuesDelegate<T> del)
			: base()
		{
			_stateData = stateData;
			_getWindowCenterValuesDelegate = del;
		}
		private CalculatedVoiLutLinearState()
		{
		}

		protected T StateData
		{
			get { return _stateData; }
		}

		protected GetWindowCenterValuesDelegate<T> GetValuesDelegate
		{
			get { return _getWindowCenterValuesDelegate; }
		}

		public override double WindowWidth
		{
			get
			{
				double windowWidth, windowCenter;
				_getWindowCenterValuesDelegate(_stateData, out windowWidth, out windowCenter);
				return windowWidth;
			}
			set
			{
				double windowWidth, windowCenter;
				_getWindowCenterValuesDelegate(_stateData, out windowWidth, out windowCenter);
				this.OwnerLut.State = new StandardVoiLutLinearState(windowWidth, windowCenter);
				this.OwnerLut.WindowWidth = value;
			}
		}

		public override double WindowCenter
		{
			get
			{
				double windowWidth, windowCenter;
				_getWindowCenterValuesDelegate(_stateData, out windowWidth, out windowCenter);
				return windowCenter;
			}
			set
			{
				double windowWidth, windowCenter;
				_getWindowCenterValuesDelegate(_stateData, out windowWidth, out windowCenter);
				this.OwnerLut.State = new StandardVoiLutLinearState(windowWidth, windowCenter);
				this.OwnerLut.WindowCenter = value;
			}
		}
	}
}
