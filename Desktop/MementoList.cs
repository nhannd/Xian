using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Summary description for MementoList.
	/// </summary>
	public class MementoList : IEnumerable<IMemento> 
	{
		private List<IMemento> m_Mementos = new List<IMemento>();

		public MementoList()
		{
		}

		public int Count
		{
			get { return m_Mementos.Count; }
		}

		public IMemento this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, m_Mementos.Count - 1, this);
				return m_Mementos[index] as IMemento;
			}
		}

		public void AddMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");

			m_Mementos.Add(memento);
		}

		#region IEnumerable<IMemento> Members

		public IEnumerator<IMemento> GetEnumerator()
		{
			return m_Mementos.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_Mementos.GetEnumerator();
		}

		#endregion
	}
}
