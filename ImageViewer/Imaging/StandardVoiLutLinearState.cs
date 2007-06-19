using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class StandardVoiLutLinearState : VoiLutLinearState
	{
		private double _windowWidth;
		private double _windowCenter;

		public StandardVoiLutLinearState()
			: this(1, 0)
		{
		}

		public StandardVoiLutLinearState(double windowWidth, double windowCenter)
			: base()
		{
			_windowWidth = Math.Max(1, windowWidth);
			_windowCenter = windowCenter;
		}

		public override double WindowWidth
		{
			get
			{
				return _windowWidth;
			}
			set
			{
				if (value < 1)
					value = 1;

				_windowWidth = value;
			}
		}

		public override double WindowCenter
		{
			get
			{
				return _windowCenter;
			}
			set
			{
				_windowCenter = value;
			}
		}

		public override IMemorableComposableLutMemento SnapshotMemento()
		{
			return new StandardVoiLutLinearState(this.WindowWidth, this.WindowCenter);
		}

		public override bool Equals(IVoiLutLinearState other)
		{
			if (other == null)
				return false;

			StandardVoiLutLinearState otherState = other as StandardVoiLutLinearState;
			if (otherState == null)
				return false;

			return (otherState.WindowCenter == this._windowCenter && otherState.WindowWidth == this.WindowWidth);
		}
	}
}
