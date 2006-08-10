using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class GrayscaleLUT : LUT, IGrayscaleLUT
	{
		// Protected attributes
		protected int _MinInputValue;
		protected int _MaxInputValue;
		protected int _MinOutputValue;
		protected int _MaxOutputValue;

		// Constructor
		public GrayscaleLUT(
			int minInputValue,
			int maxInputValue) : base(maxInputValue - minInputValue + 1)
		{
			_MinInputValue = minInputValue;
			_MaxInputValue = maxInputValue;
		}

		#region IGrayscaleLUT Members

		public int MinInputValue
		{
			get { return _MinInputValue; }
		}

		public int MaxInputValue
		{
			get { return _MaxInputValue; }
		}

		public int MinOutputValue
		{
			get { return _MinOutputValue; }
		}

		public int MaxOutputValue
		{
			get { return _MaxOutputValue; }
		}

		#endregion

		// Indexer
		public override int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, _MinInputValue, _MaxInputValue, this);
				return base.Table[index - _MinInputValue];

				/*if (index >= 0)
					return _Table[index - _MinInputValue];
				else
					return _Table[index + _NumEntries];*/
			}
			set
			{
				Platform.CheckIndexRange(index, _MinInputValue, _MaxInputValue, this);
				base.Table[index - _MinInputValue] = value;

				/*if (index >= 0)
					_Table[index - _MinInputValue] = value;
				else
					_Table[index + _NumEntries] = value;*/
			}
		}
	}
}
