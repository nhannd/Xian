using System;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Imaging
{
	public class GrayscaleLUT : LUT, IGrayscaleLUT
	{
		// Protected attributes
		protected int m_MinInputValue;
		protected int m_MaxInputValue;
		protected int m_MinOutputValue;
		protected int m_MaxOutputValue;

		// Constructor
		public GrayscaleLUT(
			int minInputValue,
			int maxInputValue) : base(maxInputValue - minInputValue + 1)
		{
			m_MinInputValue = minInputValue;
			m_MaxInputValue = maxInputValue;
		}

		#region IGrayscaleLUT Members

		public int MinInputValue
		{
			get { return m_MinInputValue; }
		}

		public int MaxInputValue
		{
			get { return m_MaxInputValue; }
		}

		public int MinOutputValue
		{
			get { return m_MinOutputValue; }
		}

		public int MaxOutputValue
		{
			get { return m_MaxOutputValue; }
		}

		#endregion

		// Indexer
		public override int this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, m_MinInputValue, m_MaxInputValue, this);
				return base.Table[index - m_MinInputValue];

				/*if (index >= 0)
					return m_Table[index - m_MinInputValue];
				else
					return m_Table[index + m_NumEntries];*/
			}
			set
			{
				Platform.CheckIndexRange(index, m_MinInputValue, m_MaxInputValue, this);
				base.Table[index - m_MinInputValue] = value;

				/*if (index >= 0)
					m_Table[index - m_MinInputValue] = value;
				else
					m_Table[index + m_NumEntries] = value;*/
			}
		}
	}
}
