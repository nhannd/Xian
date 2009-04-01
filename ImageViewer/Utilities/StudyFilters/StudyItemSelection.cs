using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public class StudyItemSelection : ICollection<StudyItem>
	{
		private readonly ICollection<StudyItem> _master;
		private readonly List<StudyItem> _innerList = new List<StudyItem>();
		private event EventHandler _selectionChanged;
		private bool _suspendEvents = false;

		internal StudyItemSelection(ICollection<StudyItem> master)
		{
			_master = master;
		}

		public event EventHandler SelectionChanged
		{
			add { _selectionChanged += value; }
			remove { _selectionChanged -= value; }
		}

		public void SuspendEvents()
		{
			_suspendEvents = true;
		}

		public void ResumeEvents(bool triggerEventImmediately)
		{
			_suspendEvents = false;

			if (triggerEventImmediately)
				NotifySelectionChanged();
		}

		protected void NotifySelectionChanged()
		{
			if (!_suspendEvents)
				EventsHelper.Fire(_selectionChanged, this, new EventArgs());
		}

		public void Add(StudyItem item)
		{
			if (!_innerList.Contains(item) && _master.Contains(item))
			{
				_innerList.Add(item);
				NotifySelectionChanged();
			}
		}

		public void Clear()
		{
			if (_innerList.Count > 0)
			{
				_innerList.Clear();
				NotifySelectionChanged();
			}
		}

		public bool Contains(StudyItem item)
		{
			return _innerList.Contains(item);
		}

		public void CopyTo(StudyItem[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _innerList.Count; }
		}

		bool ICollection<StudyItem>.IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(StudyItem item)
		{
			if (_innerList.Remove(item))
			{
				NotifySelectionChanged();
				return true;
			}
			return false;
		}

		public IEnumerator<StudyItem> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}