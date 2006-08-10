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
		private List<IMemento> _Mementos = new List<IMemento>();

		public MementoList()
		{
		}

		public int Count
		{
			get { return _Mementos.Count; }
		}

		public IMemento this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, _Mementos.Count - 1, this);
				return _Mementos[index] as IMemento;
			}
		}

		public void AddMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");

			_Mementos.Add(memento);
		}

		#region IEnumerable<IMemento> Members

		public IEnumerator<IMemento> GetEnumerator()
		{
			return _Mementos.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Mementos.GetEnumerator();
		}

		#endregion
	}
}
