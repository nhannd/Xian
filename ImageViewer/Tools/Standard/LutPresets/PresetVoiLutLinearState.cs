using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	/// <summary>
	/// This class is very simple, inheriting from <see cref="StandardVoiLutLinearState"/>.  The only reason for its existence,
	/// is so that one could add the Lut Preset's name to the text overlay, for example.
	/// </summary>
	internal class PresetVoiLutLinearState : StandardVoiLutLinearState
	{
		private string _name;

		public PresetVoiLutLinearState(string name, double windowWidth, double windowCenter)
			: base()
		{
			_name = name;
			base.WindowWidth = windowWidth;
			base.WindowCenter = windowCenter;
		}

		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// When set externally, this class will transition the <see cref="OwnerLut"/>'s state to <see cref="StandardVoiLutLinearState"/>.
		/// </summary>
		public override double WindowWidth
		{
			set
			{
				this.OwnerLut.State = new StandardVoiLutLinearState(value, this.WindowCenter);
			}
		}

		/// <summary>
		/// When set externally, this class will transition the <see cref="OwnerLut"/>'s state to <see cref="StandardVoiLutLinearState"/>.
		/// </summary>
		public override double WindowCenter
		{
			set
			{
				this.OwnerLut.State = new StandardVoiLutLinearState(this.WindowWidth, value);
			}
		}

		public override IMemorableComposableLutMemento SnapshotMemento()
		{
			return new PresetVoiLutLinearState(_name, this.WindowWidth, this.WindowCenter);
		}

		public override bool Equals(IVoiLutLinearState other)
		{
			if (other == null)
				return false;

			PresetVoiLutLinearState otherState = other as PresetVoiLutLinearState;
			if (otherState == null)
				return false;

			return (otherState.Name == this.Name && base.Equals(otherState));
		}
	}
}


