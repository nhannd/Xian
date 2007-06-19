using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a linear VOI LUT.  Unlike <see cref="StatefulVoiLutLinear"/>, this implementation
	/// is very basic and has no concept of state.  Also, there is nothing that can be accomplished using this class that cannot be
	/// done with the <see cref="StatefulVoiLutLinear"/>.  It has mainly been used for testing purposes, but serves as an example 
	/// of how the <see cref="IMemorableComposableLut"/>/<see cref="IMemorableComposableLutMemento"/> pattern can be used 
	/// to set/restore state where the mementos are actually lightweight calculated Luts.
	/// </summary>
	public sealed class StandardVoiLutLinear :
		VoiLutLinearStateBase,
		IMemorableComposableLut,
		IMemorableComposableLutMemento
	{
		private double _windowWidth = 1;
		private double _windowCenter = 0;

		public StandardVoiLutLinear(int minInputValue, int maxInputValue)
			: base(minInputValue, maxInputValue)
		{
		}

		/// <summary>
		/// Gets or sets the window width.
		/// </summary>
		public override double WindowWidth
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
		public override double WindowCenter
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

		#region IMemorableComposableLutMemento Members

		public IMemorableComposableLut RestoreLut(int minInputValue, int maxInputValue)
		{
			StandardVoiLutLinear lut = new StandardVoiLutLinear(minInputValue, maxInputValue);
			lut.WindowWidth = this.WindowWidth;
			lut.WindowCenter = this.WindowCenter;
			return lut;
		}

		#endregion

		#region IMemorableComposableLut Members

		/// <summary>
		/// Tries to restore the state of the object.  This function will not throw an exception.
		/// </summary>
		/// <param name="memento"></param>
		public bool TrySetMemento(IMemorableComposableLutMemento memento)
		{
			StandardVoiLutLinear lut = memento as StandardVoiLutLinear;
			if (lut == null)
				return false;

			this.WindowWidth = lut.WindowWidth;
			this.WindowCenter = lut.WindowCenter;

			return true;
		}

		/// <summary>
		/// Captures the state of the object.
		/// </summary>
		/// <returns></returns>
		public IMemorableComposableLutMemento CreateMemento()
		{
			StandardVoiLutLinear lut = new StandardVoiLutLinear(this.MinInputValue, this.MaxOutputValue);
			lut.WindowWidth = this.WindowWidth;
			lut.WindowCenter = this.WindowCenter;
			return lut;
		}

		/// <summary>
		/// Restores the state of the object.  Will throw an exception if the memento type is unrecognized.
		/// </summary>
		/// <param name="memento"></param>
		public void SetMemento(IMemorableComposableLutMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			StandardVoiLutLinear lut = memento as StandardVoiLutLinear;
			Platform.CheckForInvalidCast(lut, "memento", "StandardVoiLutLinear");

			this.WindowWidth = lut.WindowWidth;
			this.WindowCenter = lut.WindowCenter;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj == null)
				return false;

			StandardVoiLutLinear other = obj as StandardVoiLutLinear;
			if (other == null)
				return false;

			return (_windowCenter == other.WindowCenter && _windowWidth == other.WindowWidth);
		}
	}
}
