using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class CalculatedVoiLutLinear : VoiLutLinearBase, IVoiLutLinear
	{
		public CalculatedVoiLutLinear()
		{
		}

		protected sealed override double GetWindowWidth()
		{
			return this.WindowWidth;
		}

		protected sealed override double GetWindowCenter()
		{
			return this.WindowCenter;
		}
		
		#region IVoiLutLinear Members

		public abstract double WindowWidth { get; }
		public abstract double WindowCenter { get; }

		#endregion
	}
}
