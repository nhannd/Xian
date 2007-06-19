using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public sealed class HeaderVoiLutLinearState : CalculatedVoiLutLinearState<int>
	{
		public HeaderVoiLutLinearState(int headerIndex, GetWindowCenterValuesDelegate<int> del)
			: base(headerIndex, del)
		{
		}

		public int HeaderLutIndex
		{
			get { return base.StateData; }
		}

		public override IMemorableComposableLutMemento SnapshotMemento()
		{
			return new HeaderVoiLutLinearState(base.StateData, base.GetValuesDelegate);
		}

		public override bool Equals(IVoiLutLinearState other)
		{
			if (other == null)
				return false;

			HeaderVoiLutLinearState otherState = other as HeaderVoiLutLinearState;
			if (otherState == null)
				return false;

			return (otherState.HeaderLutIndex == this.HeaderLutIndex);
		}
	}
}
