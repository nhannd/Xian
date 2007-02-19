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
			Platform.CheckPositive(numEntries, "numEntries");
			_numEntries = numEntries;
		}

		// Properties
		public int NumEntries
		{
			get { return _numEntries; }
		}

		protected int[] Table
		{
			get
			{
				if (_table == null)
					_table = new int[_numEntries];

				return _table;
			}
		}

		// Indexer
		public virtual int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, _numEntries - 1, this);
				return this.Table[index];
			}
			set
			{
				Platform.CheckIndexRange(index, 0, _numEntries - 1, this);
				this.Table[index] = value;
			}
		}
	}
}
