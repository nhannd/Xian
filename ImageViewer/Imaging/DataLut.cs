using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class DataLut : Lut
	{
		private int _firstMappedPixelValue;
		private uint _length;
		private int _minimumOutputValue;
		private int _maximumOutputValue;

		private int[] _data;

		public DataLut(int firstMappedPixelValue, uint length, int minimumOutputValue, int maximumOutputValue)
		{
			_firstMappedPixelValue = firstMappedPixelValue;
			_length = length;

			_minimumOutputValue = minimumOutputValue;
			_maximumOutputValue = maximumOutputValue;
		}

		public DataLut(int minimumInputValue, int maximumInputValue, int minimumOutputValue, int maximumOutputValue)
			: this(minimumInputValue, (uint)(maximumInputValue - minimumInputValue + 1), minimumOutputValue, maximumOutputValue)
		{
		}

		protected int[] Data
		{ 
			get
			{
				if (_data == null)
					_data = new int[_length];

				return _data;
			}
		}

		public override int this[int index]
		{
			get
			{
				if (_data == null)
				{
					CreateLut();
					OnLutChanged();
				}

				if (index < _firstMappedPixelValue)
					return _data[0];
				else if (index >= _firstMappedPixelValue + _length)
					return _data[_length - 1];
				else
					return _data[index - _firstMappedPixelValue];
			}
			protected set
			{
				if (index < _firstMappedPixelValue || index >= _firstMappedPixelValue + _length)
					return;

				this.Data[index - _firstMappedPixelValue] = value;
			}
		}

		protected abstract void CreateLut();

		public int FirstMappedPixelValue
		{
			get { return _firstMappedPixelValue; }
		}

		public uint Length
		{
			get { return _length; }
		}

		public override int MinInputValue
		{
			get { return _firstMappedPixelValue; }
		}

		public override int MaxInputValue
		{
			get { return _firstMappedPixelValue + (int)_length - 1; }
		}

		public override int MinOutputValue
		{
			get { return _minimumOutputValue; }
		}

		public override int MaxOutputValue
		{
			get { return _maximumOutputValue; }
		}
	}
}
