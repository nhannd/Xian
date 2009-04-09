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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing the base implementation for Data Luts that are purely generated.
	/// </summary>
	/// <remarks>
	/// Often, Linear Luts are created by deriving from this class to improve performance so that
	/// the calculation is only performed once.  For an example, see <see cref="ModalityLutLinear"/>.
	/// </remarks>
	/// <seealso cref="DataLut"/>
	/// <seealso cref="IGeneratedDataLut"/>
	[Cloneable(true)]
	public abstract class GeneratedDataLut : DataLut, IGeneratedDataLut
	{
		[CloneIgnore]//data will be re-generated.
		private int[] _data;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected GeneratedDataLut()
		{
		}

		/// <summary>
		/// Since the data lut is generated, simply returns <see cref="IComposableLut.MinInputValue"/>.
		/// </summary>
		public sealed override int FirstMappedPixelValue
		{
			get { return base.MinInputValue; }
		}

		/// <summary>
		/// Since the data lut is generated, simply returns <see cref="IComposableLut.MaxInputValue"/>.
		/// </summary>
		public sealed override int LastMappedPixelValue
		{
			get { return base.MaxInputValue; }
		}

		/// <summary>
		/// Gets the Lut's data, lazily created.
		/// </summary>
		public sealed override int[] Data
		{ 
			get
			{
				if (_data == null)
				{
					_data = new int[base.Length];
					Create();
					base.OnLutChanged();
				}

				return _data;
			}
		}

		/// <summary>
		/// Looks up and returns a value at a particular index in the Lut.
		/// </summary>
		public sealed override int this[int index]
		{
			get
			{
				return base[index];
			}
			protected set
			{
				base[index] = value;
			}
		}

		/// <summary>
		/// Inheritors must implement this method and populate the Lut using an algorithm.
		/// </summary>
		protected abstract void Create();

		/// <summary>
		/// Fires the <see cref="ComposableLut.LutChanged"/> event.
		/// </summary>
		/// <remarks>
		/// Inheritors should call this method when any property of the Lut has changed.
		/// </remarks>
		protected override void OnLutChanged()
		{
			Clear();
		}

		#region IGeneratedDataLut Members

		/// <summary>
		/// Clears the data in the Lut; the Lut can be recreated at will by calling <see cref="Create"/>.
		/// </summary>
		public void Clear()
		{
			_data = null;
			base.OnLutChanged();
		}

		#endregion
	}
}
