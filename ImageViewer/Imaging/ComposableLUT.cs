using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class ComposableLUT : LUT, IComposableLUT
	{
		// Protected attributes
		protected int _minInputValue;
		protected int _maxInputValue;
		protected int _minOutputValue;
		protected int _maxOutputValue;

		// Constructor
		public ComposableLUT(
			int minInputValue,
			int maxInputValue) : base(maxInputValue - minInputValue + 1)
		{
			_minInputValue = minInputValue;
			_maxInputValue = maxInputValue;
		}

		#region IGrayscaleLUT Members

		public int MinInputValue
		{
			get { return _minInputValue; }
		}

		public int MaxInputValue
		{
			get { return _maxInputValue; }
		}

		public int MinOutputValue
		{
			get { return _minOutputValue; }
		}

		public int MaxOutputValue
		{
			get { return _maxOutputValue; }
		}

		#endregion

		// Indexer
		public override int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, _minInputValue, _maxInputValue, this);
				return base.Table[index - _minInputValue];
			}
			set
			{
				Platform.CheckIndexRange(index, _minInputValue, _maxInputValue, this);
				base.Table[index - _minInputValue] = value;
			}
		}
	}
}
