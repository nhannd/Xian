using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Summary description for LUT.
	/// </summary>
	public class LUT : ILUT
	{
		// Protected attributes
		private int _NumEntries;
		private int[] _Table;

		// Constructors
		public LUT(int numEntries)
		{
			CreateLUT(numEntries);
		}

		// Properties
		public int NumEntries
		{
			get { return _NumEntries; }
		}

		protected int[] Table
		{
			get { return _Table; }
		}

		// Indexer
		public virtual int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, _NumEntries - 1, this);
				return _Table[index];
			}
			set
			{
				Platform.CheckIndexRange(index, 0, _NumEntries - 1, this);
				_Table[index] = value;
			}
		}

		// Private method
		private void CreateLUT(int numEntries)
		{
			if (_Table == null)
			{
				Platform.CheckPositive(numEntries, "numEntries");

				_NumEntries = numEntries;
				_Table = new int[_NumEntries];
			}
		}
	}
}
