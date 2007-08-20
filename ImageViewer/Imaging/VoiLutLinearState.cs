using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

/*
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Rather than use <see cref="StandardVoiLutLinear"/>-derived classes and constantly having to change the actual
	/// VoiLut object (in whatever object owns/contains it) we use a state object for a few reasons:
	///   - the state object can detect when the Lut data <see cref="int IComposableLUT.IComposableLUT[int]"/> is about 
	///     to be accessed for the first time and run a calculation to determine the window/level.  This
	///     algorithm could be literally anything; a histogram computation, min/max pixel value, etc.  This
	///     also allows the calculation to be delayed until necessary.
	///   - We can make the state object a <see cref="IMemorableComposableLutMemento"/> so that the state
	///     of the <see cref="StatefulVoiLutLinear"/> can be remembered/restored for undo/redo operations.  Also,
	///     the <see cref="IMemorableComposableLut"/> enforces the fact that the memento must be
	///     capable of recreating the entire Lut in case the current lut type is not the same as the one
	///     being restored.
	///   - A <see cref="StatefulVoiLutLinear"/> is simple, and just has WindowWidth and WindowCenter values; its
	///     behaviour doesn't change.  The only thing you can really do with it is to run an algorithm
	///     to determine the initial w/l which is what these state objects allow you to do.
	/// </summary>
	/// <remarks>
	/// This may seem a bit overkill for linear luts, but for data Luts it makes very good sense
	/// because in most situations the memento can contain enough information to fully restore a Data Lut
	/// without actually storing the Lut itself (the byte array).  For example, most often Data Luts come
	/// from the Dicom Header, a file, an algorithm, and the data can usually be restored rather than stored
	/// in the memento.
	/// </remarks>
	public abstract class VoiLutLinearState : IVoiLutLinearState
	{
		private IStatefulVoiLutLinear _ownerLut;

		public VoiLutLinearState()
		{
		}

		public IStatefulVoiLutLinear OwnerLut
		{
			get { return _ownerLut; }
			set
			{
				if (_ownerLut == value)
					return;

				Platform.CheckForNullReference(value, "value");
				_ownerLut = value;
			}
		}

		public abstract double WindowWidth { get; set; }

		public abstract double WindowCenter { get; set; }

		public abstract IMemorableComposableLutMemento SnapshotMemento();

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj == null)
				return false;

			IVoiLutLinearState other = obj as IVoiLutLinearState;
			if (other == null)
				return false;

			return this.Equals(other);
		}

		#region IEquatable<IVoiLutLinearState> Members

		public abstract bool Equals(IVoiLutLinearState other);

		#endregion
		
		#region IMemorableComposableLutMemento Members

		public virtual IMemorableComposableLut RestoreLut(int minInputValue, int maxInputValue)
		{
			return new StatefulVoiLutLinear(this, minInputValue, maxInputValue);
		}

		#endregion
	}
}
*/