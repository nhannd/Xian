using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Summary description for CalculatedGrayscaleLUT.
	/// </summary>
	public abstract class CalculatedGrayscaleLUT : IGrayscaleLUT
	{
		protected int _minInputValue;
		protected int _maxInputValue;
		protected int _minOutputValue;
		protected int _maxOutputValue;

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
		}

		public int MaxInputValue
		{
			get	{ return _maxInputValue; }
		}

		public int MinOutputValue
		{
			get	{ return _minOutputValue; }
		}

		public int MaxOutputValue
		{
			get { return _maxOutputValue; }
		}

		#endregion
	}
}
