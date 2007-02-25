using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class CalculatedGrayscaleLUT : IComposableLUT
	{
		private int _minInputValue;
		private int _maxInputValue;
		private int _minOutputValue;
		private int _maxOutputValue;

		public CalculatedGrayscaleLUT()
		{
		}

		#region ILUT Members

		public int NumEntries
		{
			get	
			{ 
				return _maxInputValue - _minInputValue + 1; 
			}
		}

		public abstract int this[int index]
		{
			get;
			set;
		}

		#endregion

		#region IGrayscaleLUT Members

		public int MinInputValue
		{
			get { return _minInputValue; }
			protected set { _minInputValue = value; }
		}

		public int MaxInputValue
		{
			get	{ return _maxInputValue; }
			protected set { _maxInputValue = value; }
		}

		public int MinOutputValue
		{
			get	{ return _minOutputValue; }
			protected set { _minOutputValue = value; }
		}

		public int MaxOutputValue
		{
			get { return _maxOutputValue; }
			protected set { _maxOutputValue = value; }
		}

		#endregion
	}
}
