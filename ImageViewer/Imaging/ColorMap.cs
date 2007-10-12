#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements a Color Map as a LUT.
	/// </summary>
	/// <remarks>
	/// The values in the LUT represent ARGB values that are used 
	/// by the <see cref="IRenderer"/> to display the image.
	/// </remarks>
	public abstract class ColorMap : GeneratedDataLut, IColorMap
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ColorMap"/>.
		/// </summary>
		protected ColorMap()
		{
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		public sealed override int MinOutputValue
		{
			get
			{
				throw new MemberAccessException(SR.ExceptionColorMapCannotHaveMinimumOutputValue);
			}
			protected set
			{
				throw new MemberAccessException(SR.ExceptionColorMapCannotHaveMinimumOutputValue);
			}
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		public sealed override int MaxOutputValue
		{
			get
			{
				throw new MemberAccessException(SR.ExceptionColorMapCannotHaveMaximumOutputValue);
			}
			protected set
			{
				throw new MemberAccessException(SR.ExceptionColorMapCannotHaveMaximumOutputValue);
			}
		}

		#region IColorMap Members

		public new int[] Data
		{
			get
			{
				if (!Created)
					Create();

				return base.Data;
			}
		}

		#endregion

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns>A 32-bit ARGB value.</returns>
		public sealed override int this[int index]
		{
			get
			{
				return base[index];
			}
			protected  set
			{
				base[index] = value;
			}
		}

		/// <summary>
		/// Should be called by implementors when the Lut has changed.
		/// </summary>
		protected sealed override void OnLutChanged()
		{
			base.OnLutChanged();
		}

		/// <summary>
		/// Gets a string key that identifies this particular LUT's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with *equality*, since some Luts can be
		/// dependent upon the actual image to which it belongs.  The method should simply 
		/// be used to determine if a lut in the <see cref="ComposedLutPool"/> is the same 
		/// as an existing one.
		/// </remarks>
		public sealed override string GetKey()
		{
			return String.Format("{0}_{1}_{2}",
				this.MinInputValue,
				this.MaxInputValue,
				this.GetType().ToString());
		}

		public sealed override IMemento CreateMemento()
		{
			return base.CreateMemento();
		}

		public sealed override void SetMemento(IMemento memento)
		{
			base.SetMemento(memento);
		}

		#region IEquatable<IColorMap> Members

		public bool Equals(IColorMap other)
		{
			return this.MinInputValue == other.MinInputValue && 
				this.MaxInputValue == other.MaxInputValue &&
				this.GetType() == other.GetType();
		}

		#endregion
	}
}
