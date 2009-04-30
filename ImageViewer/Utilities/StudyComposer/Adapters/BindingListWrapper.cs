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
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters
{
	public class BindingListWrapper<T> : IBindingList {
		private static readonly BindingList<T> EMPTY_BINDING_LIST = new BindingList<T>(new List<T>(0));
		private event ListChangedEventHandler _listChanged;
		private BindingList<T> _internalList;

		internal BindingListWrapper() : this(EMPTY_BINDING_LIST) { }
		internal BindingListWrapper(BindingList<T> bindingList) {
			if (bindingList == null)
				bindingList = EMPTY_BINDING_LIST;

			_internalList = bindingList;
			_internalList.ListChanged += OnListChanged;
		}

		~BindingListWrapper() {
			_internalList.ListChanged -= OnListChanged;
		}

		internal BindingList<T> InternalList {
			get { return _internalList; }
			set {
				// internally, render a null internal list as an empty list
				if (value == null)
					value = EMPTY_BINDING_LIST;
				if (_internalList != value) {
					_internalList.ListChanged -= OnListChanged;
					_internalList = value;
					_internalList.ListChanged += OnListChanged;
					OnListChanged(null, new ListChangedEventArgs(ListChangedType.Reset, -1));
				}
			}
		}

		void OnListChanged(object sender, ListChangedEventArgs e) {
			if (_listChanged != null) {
				_listChanged(this, e);
			}
		}

		#region IBindingList Members and Strongly-Typed Overloads

		void IBindingList.AddIndex(PropertyDescriptor property) {
			((IBindingList)_internalList).AddIndex(property);
		}

		public object AddNew() {
			return _internalList.AddNew();
		}

		public bool AllowEdit {
			get { return _internalList.AllowEdit; }
		}

		public bool AllowNew {
			get { return _internalList.AllowNew; }
		}

		public bool AllowRemove {
			get { return _internalList.AllowRemove; }
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			((IBindingList)_internalList).ApplySort(property, direction);
		}

		int IBindingList.Find(PropertyDescriptor property, object key) {
			return ((IBindingList)_internalList).Find(property, key);
		}

		bool IBindingList.IsSorted {
			get { return ((IBindingList)_internalList).IsSorted; }
		}

		public event ListChangedEventHandler ListChanged {
			add { _listChanged += value; }
			remove { _listChanged -= value; }
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			((IBindingList)_internalList).RemoveIndex(property);
		}

		void IBindingList.RemoveSort() {
			((IBindingList)_internalList).RemoveSort();
		}

		ListSortDirection IBindingList.SortDirection {
			get { return ((IBindingList)_internalList).SortDirection; }
		}

		PropertyDescriptor IBindingList.SortProperty {
			get { return ((IBindingList)_internalList).SortProperty; }
		}

		bool IBindingList.SupportsChangeNotification {
			get { return ((IBindingList)_internalList).SupportsChangeNotification; }
		}

		bool IBindingList.SupportsSearching {
			get { return ((IBindingList)_internalList).SupportsSearching; }
		}

		bool IBindingList.SupportsSorting {
			get { return ((IBindingList)_internalList).SupportsSorting; }
		}

		int IList.Add(object value) {
			return this.Add((T)value);
		}

		public int Add(T value) {
			return ((IBindingList)_internalList).Add(value);
		}

		public void Clear() {
			_internalList.Clear();
		}

		bool IList.Contains(object value) {
			return this.Contains((T)value);
		}

		public bool Contains(T value) {
			return _internalList.Contains(value);
		}

		int IList.IndexOf(object value) {
			return this.IndexOf((T)value);
		}

		public int IndexOf(T value) {
			return _internalList.IndexOf(value);
		}

		void IList.Insert(int index, object value) {
			this.Insert(index, (T)value);
		}

		public void Insert(int index, T value) {
			_internalList.Insert(index, value);
		}

		bool IList.IsFixedSize {
			get { return ((IBindingList)_internalList).IsFixedSize; }
		}

		bool IList.IsReadOnly {
			get { return ((IBindingList)_internalList).IsReadOnly; }
		}

		void IList.Remove(object value) {
			this.Remove((T)value);
		}

		public void Remove(T value) {
			_internalList.Remove(value);
		}

		public void RemoveAt(int index) {
			_internalList.RemoveAt(index);
		}

		object IList.this[int index] {
			get { return this[index]; }
			set { this[index] = (T)value; }
		}

		public T this[int index] {
			get { return _internalList[index]; }
			set { _internalList[index] = value; }
		}

		void ICollection.CopyTo(Array array, int index) {
			this.CopyTo((T[])array, index);
		}

		public void CopyTo(T[] array, int index) {
			_internalList.CopyTo(array, index);
		}

		public int Count {
			get { return _internalList.Count; }
		}

		bool ICollection.IsSynchronized {
			get { return ((IBindingList)_internalList).IsSynchronized; }
		}

		object ICollection.SyncRoot {
			get { return ((IBindingList)_internalList).SyncRoot; }
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _internalList.GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator() {
			return _internalList.GetEnumerator();
		}

		#endregion
	}
}