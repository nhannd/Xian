using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	internal interface IStudyFilterColumnCollectionCallbacks
	{
		void ColumnInserted(int index, StudyFilterColumn newColumn);
		void ColumnRemoved(int index, StudyFilterColumn oldColumn);
		void ColumnChanged(int index, StudyFilterColumn oldColumn, StudyFilterColumn newColumn);
		void ColumnsChanged(IEnumerable<StudyFilterColumn> newColumns);
	}

	public class StudyFilterColumnCollection : IList<StudyFilterColumn>
	{
		private readonly List<StudyFilterColumn> _innerList = new List<StudyFilterColumn>();
		private readonly IStudyFilterColumnCollectionCallbacks _callback;

		internal StudyFilterColumnCollection(IStudyFilterColumnCollectionCallbacks callback)
		{
			_callback = callback;
		}

		public StringCollection Serialize()
		{
			StringCollection columns = new StringCollection();
			foreach (StudyFilterColumn column in _innerList)
			{
				columns.Add(column.Key);
			}
			return columns;
		}

		public void Deserialize(StringCollection columns)
		{
			if (columns != null)
			{
				_innerList.Clear();
				foreach (string columnKey in columns)
				{
					StudyFilterColumn column = StudyFilterColumn.GetColumn(columnKey);
					if (column != null)
						_innerList.Add(column);
				}
				_callback.ColumnsChanged(_innerList);
			}
		}

		public int IndexOf(StudyFilterColumn item)
		{
			return _innerList.IndexOf(item);
		}

		public void Insert(int index, StudyFilterColumn item)
		{
			if (_innerList.Contains(item))
				throw new ArgumentException("Column already exists in collection.", "item");
			_innerList.Insert(index, item);
			_callback.ColumnInserted(index, item);
		}

		public void RemoveAt(int index)
		{
			StudyFilterColumn item = _innerList[index];
			_innerList.RemoveAt(index);
			_callback.ColumnRemoved(index, item);
		}

		public StudyFilterColumn this[int index]
		{
			get { return _innerList[index]; }
			set
			{
				if (_innerList.Contains(value))
					throw new ArgumentException("Column already exists in collection.", "value");

				StudyFilterColumn item = _innerList[index];
				_innerList[index] = value;
				_callback.ColumnChanged(index, item, value);
			}
		}

		public void Add(StudyFilterColumn item)
		{
			if (_innerList.Contains(item))
				throw new ArgumentException("Column already exists in collection.", "item");
			int index = _innerList.Count;
			_innerList.Add(item);
			_callback.ColumnInserted(index, item);
		}

		public void Clear()
		{
			_innerList.Clear();
			_callback.ColumnsChanged(this);
		}

		public bool Contains(StudyFilterColumn item)
		{
			return _innerList.Contains(item);
		}

		void ICollection<StudyFilterColumn>.CopyTo(StudyFilterColumn[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _innerList.Count; }
		}

		bool ICollection<StudyFilterColumn>.IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(StudyFilterColumn item)
		{
			int index = _innerList.IndexOf(item);
			if (index >= 0)
			{
				_innerList.RemoveAt(index);
				_callback.ColumnRemoved(index, item);
				return true;
			}
			return false;
		}

		public IEnumerator<StudyFilterColumn> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}