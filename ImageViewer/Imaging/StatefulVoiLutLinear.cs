using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a linear VOI LUT, but uses the state pattern to allow for
	/// things like algorithms to be performed when the Lut is first accessed, etc.
	/// </summary>
	public sealed class StatefulVoiLutLinear :
		VoiLutLinearStateBase, 
		IStatefulVoiLutLinear
	{
		private IVoiLutLinearState _state;

		public StatefulVoiLutLinear(IVoiLutLinearState state, int minInputValue, int maxInputValue)
			: base(minInputValue, maxInputValue)
		{
			this.State = state;
		}

		public StatefulVoiLutLinear(int minInputValue, int maxInputValue)
			: this(new StandardVoiLutLinearState(), minInputValue, maxInputValue)
		{
		}

		public IVoiLutLinearState State
		{
			get { return _state; }
			set
			{
				Platform.CheckForNullReference(value, "value");

				if (value == _state)
					return;

				_state = value;
				_state.OwnerLut = this;
				base.Recalculate();
			}
		}

		/// <summary>
		/// Gets or sets the window width.
		/// </summary>
		public override double WindowWidth
		{
			get { return _state.WindowWidth; }
			set
			{
				double oldValue = _state.WindowWidth;
				_state.WindowWidth = value;
				if (_state.WindowWidth != oldValue)
					base.Recalculate();
			}
		}

		/// <summary>
		/// Gets or sets the window center.
		/// </summary>
		public override double WindowCenter
		{
			get { return _state.WindowCenter; }
			set
			{
				double oldValue = _state.WindowCenter;
				_state.WindowCenter = value;
				if (_state.WindowCenter != oldValue)
					base.Recalculate();
			}
		}

		#region IMemorableComposableLut Members

		/// <summary>
		/// Tries to restore the state of the object.  This function will not throw an exception.
		/// </summary>
		/// <param name="memento"></param>
		public bool TrySetMemento(IMemorableComposableLutMemento memento)
		{
			if (memento == null)
				return false;

			IVoiLutLinearState state = memento as IVoiLutLinearState;
			if (state == null)
				return false;

			this.State = (IVoiLutLinearState)state.SnapshotMemento();
			return true;
		}

		/// <summary>
		/// Captures a 'snapshot' of the <see cref="State"/> by calling <see cref="IVoiLutLinearState.SnapshotMemento"/>.
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
			IVoiLutLinearState state = memento as IVoiLutLinearState;
			Platform.CheckForInvalidCast(state, "memento", "IVoiLutLinearState");

			this.State = (IVoiLutLinearState)state.SnapshotMemento();
		}

		#endregion 
	}
}