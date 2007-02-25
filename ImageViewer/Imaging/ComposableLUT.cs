using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class ComposableLUT : LUT, IComposableLUT
	{
		private int _minInputValue;
		private int _maxInputValue;
		private int _minOutputValue;
		private int _maxOutputValue;

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
			protected set { _minInputValue = value; }
		}

		public int MaxInputValue
		{
			get { return _maxInputValue; }
			protected set { _maxInputValue = value; }
		}

		public virtual int MinOutputValue
		{
			get { return _minOutputValue; }
			protected set { _minOutputValue = value; }
		}

		public virtual int MaxOutputValue
		{
			get { return _maxOutputValue; }
			protected set { _maxOutputValue = value; }
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
