#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public interface IStudyFilterColumnCollection : IList<StudyFilterColumn>, IEnumerable<string>, IEnumerable
	{
		StringCollection Serialize();
		void Deserialize(StringCollection columns);
		int IndexOf(string columnKey);
		StudyFilterColumn this[string columnKey] { get; }
		bool Contains(string columnKey);
	}

	partial class StudyFilterComponent
	{
		private class StudyFilterColumnCollection : IStudyFilterColumnCollection
		{
			private readonly List<StudyFilterColumn> _innerList = new List<StudyFilterColumn>();
			private readonly StudyFilterComponent _owner;

			public StudyFilterColumnCollection(StudyFilterComponent owner)
			{
				_owner = owner;
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
					foreach (StudyFilterColumn column in _innerList)
						column.Owner = null;

					_innerList.Clear();
					foreach (string columnKey in columns)
					{
						StudyFilterColumn column = StudyFilterColumn.CreateColumn(columnKey);
						if (column != null)
							_innerList.Add(column);
					}
					_owner.ColumnsChanged(_innerList);

					foreach (StudyFilterColumn column in _innerList)
						column.Owner = _owner;
				}
			}

			public int IndexOf(StudyFilterColumn item)
			{
				return _innerList.IndexOf(item);
			}

			public int IndexOf(string columnKey)
			{
				for (int n = 0; n < _innerList.Count; n++)
					if (_innerList[n].Key == columnKey)
						return n;
				return -1;
			}

			public void Insert(int index, StudyFilterColumn item)
			{
				if (_innerList.Contains(item))
					throw new ArgumentException("Column already exists in collection.", "item");
				_innerList.Insert(index, item);
				_owner.ColumnInserted(index, item);
				item.Owner = _owner;
			}

			public void RemoveAt(int index)
			{
				StudyFilterColumn item = _innerList[index];
				_innerList.RemoveAt(index);
				_owner.ColumnRemoved(index, item);
				item.Owner = null;
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
					_owner.ColumnChanged(index, item, value);
					item.Owner = null;
					value.Owner = _owner;
				}
			}

			public StudyFilterColumn this[string columnKey]
			{
				get
				{
					for (int n = 0; n < _innerList.Count; n++)
						if (_innerList[n].Key == columnKey)
							return _innerList[n];
					throw new KeyNotFoundException();
				}
			}

			public void Add(StudyFilterColumn item)
			{
				if (_innerList.Contains(item))
					throw new ArgumentException("Column already exists in collection.", "item");
				int index = _innerList.Count;
				_innerList.Add(item);
				_owner.ColumnInserted(index, item);
				item.Owner = _owner;
			}

			public void Clear()
			{
				foreach (StudyFilterColumn column in _innerList)
					column.Owner = null;

				_innerList.Clear();
				_owner.ColumnsChanged(this);

				foreach (StudyFilterColumn column in _innerList)
					column.Owner = _owner;
			}

			public bool Contains(StudyFilterColumn item)
			{
				return _innerList.Contains(item);
			}

			public bool Contains(string columnKey)
			{
				for (int n = 0; n < _innerList.Count; n++)
					if (_innerList[n].Key == columnKey)
						return true;
				return false;
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
					_owner.ColumnRemoved(index, item);
					item.Owner = null;
					return true;
				}
				return false;
			}

			public IEnumerator<StudyFilterColumn> GetEnumerator()
			{
				return _innerList.GetEnumerator();
			}

			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				foreach (StudyFilterColumn column in this)
					yield return column.Key;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}
	}
}