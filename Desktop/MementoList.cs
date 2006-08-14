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
		private List<IMemento> _mementos = new List<IMemento>();

		public MementoList()
		{
		}

		public int Count
		{
			get { return _mementos.Count; }
		}

		public IMemento this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, _mementos.Count - 1, this);
				return _mementos[index] as IMemento;
			}
		}

		public void AddMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");

			_mementos.Add(memento);
		}

		#region IEnumerable<IMemento> Members

		public IEnumerator<IMemento> GetEnumerator()
		{
			return _mementos.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _mementos.GetEnumerator();
		}

		#endregion
	}
}
