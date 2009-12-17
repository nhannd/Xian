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