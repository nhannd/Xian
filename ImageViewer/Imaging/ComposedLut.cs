#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class ComposedLut : IComposedLut
	{
		private readonly int _minInputValue;
		private readonly int _maxInputValue;
		private readonly int _length;
		private readonly int[] _data;

		public ComposedLut(LutCollection luts): this(luts, null)
		{
		}

		public ComposedLut(LutCollection luts, BufferCache<int> cache)
		{
			//luts.Validate();

			int lutCount;
			IComposableLut firstLut, lastLut;
			GetFirstAndLastLut(luts, out firstLut, out lastLut, out lutCount);

			_minInputValue = firstLut.MinInputValue;
			_maxInputValue = firstLut.MaxInputValue;
			_length = _maxInputValue - _minInputValue + 1;
			_data = cache != null ? cache.Allocate(_length) : MemoryManager.Allocate<int>(_length);

			//copy to array because accessing ObservableList's indexer in a tight loop is very expensive
			IComposableLut[] lutArray = new IComposableLut[lutCount];
			luts.CopyTo(lutArray, 0);

			unsafe
			{
				fixed (int* composedLutData = _data)
				{
					int* pLutData = composedLutData;
					int min = _minInputValue;
					int max = _maxInputValue + 1;

					for (int i = min; i < max; ++i)
					{
						int val = i;

						for (int j = 0; j < lutCount; ++j)
							val = lutArray[j][val];

						*pLutData = val;
						++pLutData;
					}
				}
			}
		}

		#region IComposedLut Members

		public int[] Data
		{
			get { return _data; }
		}

		#endregion

		#region ILut Members

		public int MinInputValue
		{
			get { return _minInputValue; }
		}

		public int MaxInputValue
		{
			get { return _maxInputValue; }
		}
        
		//never guaranteed because the output could be colour.
		public int MinOutputValue
		{
			get { throw new InvalidOperationException("Composed Luts cannot have a MinOutputValue."); }
		}
		public int MaxOutputValue
		{
			get { throw new InvalidOperationException("Composed Luts cannot have a MaxOutputValue."); }
		}

		public int this[int index]
		{
			get
			{
				if (index <= _minInputValue)
					return _data[0];
				else if (index >= _maxInputValue)
					return Data[_length - 1];

				return Data[index - _minInputValue];
			}
		}

		#endregion

		#region Private Static

		private static void GetFirstAndLastLut(IEnumerable<IComposableLut> luts, out IComposableLut firstLut, out IComposableLut lastLut, out int count)
		{
			count = 0;
			firstLut = lastLut = null;

			foreach (IComposableLut lut in luts)
			{
				if (firstLut == null)
					firstLut = lut;

				lastLut = lut;
				++count;
			}

			if (count == 0)
				throw new ArgumentException("There are no LUTs in the collection.");
		}

		#endregion
	}
}