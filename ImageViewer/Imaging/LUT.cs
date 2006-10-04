using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class LUT : ILUT
	{
		// Protected attributes
		private int _numEntries;
		private int[] _table;

		// Constructors
		public LUT(int numEntries)
		{
			CreateLUT(numEntries);
		}

		// Properties
		public int NumEntries
		{
			get { return _numEntries; }
		}

		protected int[] Table
		{
			get { return _table; }
		}

		// Indexer
		public virtual int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, _numEntries - 1, this);
				return _table[index];
			}
			set
			{
				Platform.CheckIndexRange(index, 0, _numEntries - 1, this);
				_table[index] = value;
			}
		}

		// Private method
		private void CreateLUT(int numEntries)
		{
			if (_table == null)
			{
				Platform.CheckPositive(numEntries, "numEntries");

				_numEntries = numEntries;
				_table = new int[_numEntries];
			}
		}
	}
}
