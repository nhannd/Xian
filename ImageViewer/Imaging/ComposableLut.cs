#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing base implementation for a Lut that can be added to a <see cref="LutCollection"/>.
	/// </summary>
	/// <seealso cref="IComposableLut"/>
	[Cloneable(true)]
	public abstract class ComposableLut : IComposableLut
	{
		#region Private Fields

		private event EventHandler _lutChanged;
		
		#endregion

		#region Protected Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		internal ComposableLut()
		{
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Fires the <see cref="LutChanged"/> event.
		/// </summary>
		/// <remarks>
		/// Inheritors should call this method when any property of the Lut has changed.
		/// </remarks>
		protected virtual void OnLutChanged()
		{
			EventsHelper.Fire(_lutChanged, this, EventArgs.Empty);
		}

		#endregion

		#region IComposableLut Members

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		protected abstract double MinInputValueCore { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		protected abstract double MaxInputValueCore { get; set; }

		/// <summary>
		/// Gets or sets the minimum output value.
		/// </summary>
		protected abstract double MinOutputValueCore { get; set; }

		/// <summary>
		/// Gets or sets the maximum output value.
		/// </summary>
		protected abstract double MaxOutputValueCore { get; set; }

		double IComposableLut.MinInputValue
		{
			get { return MinInputValueCore; }
			set { MinInputValueCore = value; }
		}

		double IComposableLut.MaxInputValue
		{
			get { return MaxInputValueCore; }
			set { MaxInputValueCore = value; }
		}

		double IComposableLut.MinOutputValue
		{
			get { return MinOutputValueCore; }
		}

		double IComposableLut.MaxOutputValue
		{
			get { return MaxOutputValueCore; }
		}

		/// <summary>
		/// Gets the output value of the lookup table for a given input value.
		/// </summary>
		protected abstract double Lookup(double input);

		double IComposableLut.this[double input]
		{
			get { return Lookup(input); }
		}

		/// <summary>
		/// Fired when the LUT has changed in some way.
		/// </summary>
		public event EventHandler LutChanged
		{
			add { _lutChanged += value; }
			remove { _lutChanged -= value; }
		}

		/// <summary>
		/// Gets a string key that identifies this particular Lut's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some Luts can be
		/// dependent upon the actual image to which it belongs.
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
		public virtual object CreateMemento()
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
		/// <param name="memento">The memento object from which to restore the Lut's state.</param>
		/// <exception cref="InvalidOperationException">Thrown if <paramref name="memento"/> is <B>not</B> null, 
		/// which would indicate that <see cref="CreateMemento"/> has been overridden, but <see cref="SetMemento"/> has not.</exception>
		public virtual void SetMemento(object memento)
		{
			if (memento != null)
				throw new InvalidOperationException(SR.ExceptionMustOverrideSetMemento);
		}

		#endregion

		/// <summary>
		/// Creates a deep-copy of the <see cref="IComposableLut"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IComposableLut"/>s may return null from this method when appropriate.	
		/// </remarks>
		public IComposableLut Clone()
		{
			return CloneBuilder.Clone(this) as IComposableLut;
		}
	}
}
