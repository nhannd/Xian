using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing base implementation for a LUT that can be added to a <see cref="LutCollection"/>
	/// </summary>
	public abstract class Lut : ILut
	{
		private event EventHandler _lutChanged;

		protected virtual void OnLutChanged()
		{
			EventsHelper.Fire(_lutChanged, this, EventArgs.Empty);
		}

		#region ILUT Members

		/// <summary>
		/// Gets or sets the minimum input value.  This value will be set internally by the framework.
		/// </summary>
		public abstract int MinInputValue { get; set; }

		/// <summary>
		/// Gets the maximum input value.  This value will be set internally by the framework.
		/// </summary>
		public abstract int MaxInputValue { get; set; }

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public abstract int MinOutputValue { get; protected set;}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public abstract int MaxOutputValue { get; protected set;}

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		/// <param name="index">the index into the Lut</param>
		/// <returns>the value at the given index</returns>
		public abstract int this[int index] { get; protected set; }

		/// <summary>
		/// Fired when the LUT has changed in some way.
		/// </summary>
		public event EventHandler LutChanged
		{
			add { _lutChanged += value; }
			remove { _lutChanged -= value; }
		}

		/// <summary>
		/// Gets a string key that identifies this particular LUT's characteristics, so that 
		/// an image's <see cref="OutputLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with *equality*, since some Luts can be
		/// dependent upon the actual image to which it belongs.  The method should simply 
		/// be used to determine if a lut in the <see cref="OutputLutPool"/> is the same 
		/// as an existing one.
		/// </remarks>
		public abstract string GetKey();

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		public abstract string GetDescription();

		#endregion

		#region IMemorable Members

		/// <summary>
		/// Returns null.
		/// </summary>
		/// <remarks>
		/// Override this member only when necessary.  If this method is overridden, <see cref="SetMemento"/> must also be overridden.
		///  </remarks>
		/// <returns>null, unless overridden.</returns>
		public virtual IMemento CreateMemento()
		{
			return null;
		}

		/// <summary>
		/// Does nothing unless overridden.
		/// </summary>
		/// <remarks>
		/// If you override <see cref="CreateMemento"/> to capture the Lut's state, you must also override this method
		/// to allow the state to be restored.
		/// </remarks>
		/// <exception cref="InvalidOperationException">throw if the input <see cref="IMemento"/> is <B>not</B> null, 
		/// which would indicate that <see cref="CreateMemento"/> has been overridden, but <see cref="SetMemento"/> has not.</exception>
		/// <param name="memento">the <see cref="IMemento"/> from which to restore the Lut's state.</param>
		public virtual void SetMemento(IMemento memento)
		{
			if (memento != null)
				throw new InvalidOperationException(SR.ExceptionMustOverrideSetMemento);
		}

		#endregion
	}
}
