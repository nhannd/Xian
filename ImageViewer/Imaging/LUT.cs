using System;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Imaging
{
	/// <summary>
	/// Summary description for LUT.
	/// </summary>
	public class LUT : ILUT
	{
		// Protected attributes
		private int m_NumEntries;
		private int[] m_Table;

		// Constructors
		public LUT(int numEntries)
		{
			CreateLUT(numEntries);
		}

		// Properties
		public int NumEntries
		{
			get { return m_NumEntries; }
		}

		protected int[] Table
		{
			get { return m_Table; }
		}

		// Indexer
		public virtual int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, m_NumEntries - 1, this);
				return m_Table[index];
			}
			set
			{
				Platform.CheckIndexRange(index, 0, m_NumEntries - 1, this);
				m_Table[index] = value;
			}
		}

		// Private method
		private void CreateLUT(int numEntries)
		{
			if (m_Table == null)
			{
				Platform.CheckPositive(numEntries, "numEntries");

				m_NumEntries = numEntries;
				m_Table = new int[m_NumEntries];
			}
		}
	}
}
