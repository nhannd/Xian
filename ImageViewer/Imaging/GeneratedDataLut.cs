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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing the base implementation for Data Luts that are purely generated, 
	/// usually based on an equation or algorithm.
	/// </summary>
	/// <remarks>
	/// Often, Linear Luts are created by deriving from this class to improve performance so that
	/// the calculation is only performed once.  For an example, see <see cref="ModalityLutLinear"/>.
	/// </remarks>
	public abstract class GeneratedDataLut : ComposableLut, IGeneratedDataLut
	{
		private int _minimumInputValue;
		private int _maximimInputValue;
		private int _minimumOutputValue;
		private int _maximumOutputValue;

		private int[] _data;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected GeneratedDataLut()
		{
			_minimumInputValue = int.MinValue;
			_maximimInputValue = int.MaxValue;

			_minimumOutputValue = int.MinValue;
			_maximumOutputValue = int.MaxValue;
		}

		/// <summary>
		/// Gets whether or not the underlying <see cref="Data"/> has been allocated yet.
		/// </summary>
		protected bool Created
		{
			get { return _data != null; }
		}

		/// <summary>
		/// Gets the Lut data, lazily created.
		/// </summary>
		protected int[] Data
		{ 
			get
			{
				if (_minimumInputValue == int.MinValue || _maximimInputValue == int.MaxValue)
					throw new InvalidOperationException(SR.ExceptionMinMaxInputValuesNotSet);

				if (_data == null)
					_data = new int[this.Length];

				return _data;
			}
		}

		/// <summary>
		/// Looks up and returns a value at a particular index in the Lut.
		/// </summary>
		public override int this[int index]
		{
			get
			{
				if (_data == null)
				{
					Create();
					OnLutChanged();
				}

				if (index <= _minimumInputValue)
					return _data[0];
				else if (index >= this.MaxInputValue)
					return _data[this.Length - 1];
				else
					return _data[index - this.MinInputValue];
			}
			protected set
			{
				if (index < this.MinInputValue || index > this.MaxInputValue)
					return;

				this.Data[index - this.MinInputValue] = value;
			}
		}

		/// <summary>
		/// Returns the length of the Lut.
		/// </summary>
		public uint Length
		{
			get { return (uint)(this.MaxInputValue - this.MinInputValue + 1); }
		}

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public sealed override int MinInputValue
		{
			get { return _minimumInputValue; }
			set
			{
				if (_minimumInputValue == value)
					return;

				_minimumInputValue = value;
				Clear();
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public sealed override int MaxInputValue
		{
			get { return _maximimInputValue; }
			set
			{
				if (_maximimInputValue == value)
					return;

				_maximimInputValue = value;
				Clear();
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public override int MinOutputValue
		{
			get { return _minimumOutputValue; }
			protected set
			{
				if (_minimumOutputValue == value)
					return;
				
				_minimumOutputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public override int MaxOutputValue
		{
			get { return _maximumOutputValue; }
			protected set
			{
				if (_maximumOutputValue == value)
					return;

				_maximumOutputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Inheritors must implement this method and create the Lut using their particular algorithm.
		/// </summary>
		protected abstract void Create();

		#region IDataLut Members

		/// <summary>
		/// Clears the data in the Lut; the Lut can be recreated at will by calling <see cref="Create"/>.
		/// </summary>
		public void Clear()
		{
			_data = null;
		}

		#endregion
	}
}
