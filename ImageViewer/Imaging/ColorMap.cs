#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements a Color Map as a LUT.
	/// </summary>
	/// <remarks>
	/// The values in the LUT represent ARGB values that are used 
	/// by an <see cref="ClearCanvas.ImageViewer.Rendering.IRenderer"/> to display the image.
	/// </remarks>
	/// <seealso cref="GeneratedDataLut"/>
	public abstract class ColorMap : GeneratedDataLut
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected ColorMap()
		{
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown always.</exception>
		public sealed override int MinOutputValue
		{
			get
			{
				throw new InvalidOperationException("A color map cannot have a minimum output value.");
			}
			protected set
			{
				throw new InvalidOperationException("A color map cannot have a minimum output value.");
			}
		}

		/// <summary>
		/// Not applicable.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown always.</exception>
		public sealed override int MaxOutputValue
		{
			get
			{
				throw new InvalidOperationException("A color map cannot have a maximum output value.");
			}
			protected set
			{
				throw new InvalidOperationException("A color map cannot have a maximum output value.");
			}
		}

		/// <summary>
		/// Gets a string key that identifies this particular LUT's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// You should not have to override this method.
		/// </remarks>
		public override string GetKey()
		{
			return String.Format("{0}_{1}_{2}",
				this.MinInputValue,
				this.MaxInputValue,
				this.GetType().ToString());
		}
	}
}
