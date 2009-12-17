#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
		protected ComposableLut()
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
		public abstract int MinInputValue { get; set; }

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
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
