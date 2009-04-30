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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Base implementation for <see cref="IDataLut"/>s.
	/// </summary>
	/// <remarks>
	/// Normally, you should not have to inherit directly from this class.
	/// <see cref="SimpleDataLut"/> or <see cref="GeneratedDataLut"/> should cover
	/// most, if not all, common use cases.
	/// </remarks>
	[Cloneable(true)]
	public abstract class DataLut : ComposableLut, IDataLut
	{
		private int _minInputValue;
		private int _maxInputValue;
		private int _minOutputValue;
		private int _maxOutputValue;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected DataLut()
		{
		}
		
		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public override int MinInputValue
		{
			get { return _minInputValue; }
			set
			{
				if (value == _minInputValue)
					return;

				_minInputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public override int MaxInputValue
		{
			get { return _maxInputValue; }
			set
			{
				if (value == _maxInputValue)
					return;

				_maxInputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public override int MinOutputValue
		{
			get { return _minOutputValue; }
			protected set
			{
				if (_minOutputValue == value)
					return;

				_minOutputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public override int MaxOutputValue
		{
			get { return _maxOutputValue; }
			protected set
			{
				if (value == _maxOutputValue)
					return;

				_maxOutputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		public override int this[int index]
		{
			get
			{
				Platform.CheckMemberIsSet(Data, "Data");
				Platform.CheckTrue(Data.Length == Length, "Data Lut length check");

				if (index <= FirstMappedPixelValue)
					return Data[0];
				else if (index >= LastMappedPixelValue)
					return Data[this.Length - 1];
				else
					return Data[index - this.FirstMappedPixelValue];
			}
			protected set
			{
				if (index < this.FirstMappedPixelValue || index > this.LastMappedPixelValue)
					return;

				this.Data[index - this.FirstMappedPixelValue] = value;
			}
		}

		///<summary>
		/// Gets the length of <see cref="Data"/>.
		///</summary>
		/// <remarks>
		/// The reason for this member's existence is that <see cref="Data"/> may
		/// not yet exist; this value is based solely on <see cref="IDataLut.FirstMappedPixelValue"/>
		/// and <see cref="DataLut.LastMappedPixelValue"/>.
		/// </remarks>
		public int Length
		{
			get
			{
				return 1 + LastMappedPixelValue - FirstMappedPixelValue;
			}
		}

		#region IDataLut Members

		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		public abstract int FirstMappedPixelValue { get; }

		/// <summary>
		/// Gets the last mapped pixel value.
		/// </summary>
		public abstract int LastMappedPixelValue { get; }

		/// <summary>
		/// Gets the lut data.
		/// </summary>
		public abstract int[] Data { get; }

		#endregion
	}
}
