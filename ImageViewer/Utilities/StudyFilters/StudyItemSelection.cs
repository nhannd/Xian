#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public class StudyItemSelection : ICollection<IStudyItem>
	{
		private readonly ICollection<IStudyItem> _master;
		private readonly List<IStudyItem> _innerList = new List<IStudyItem>();
		private event EventHandler _selectionChanged;
		private bool _suspendEvents = false;

		internal StudyItemSelection(ICollection<IStudyItem> master)
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

		public void Add(IStudyItem item)
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

		public bool Contains(IStudyItem item)
		{
			return _innerList.Contains(item);
		}

		public void CopyTo(IStudyItem[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _innerList.Count; }
		}

		bool ICollection<IStudyItem>.IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IStudyItem item)
		{
			if (_innerList.Remove(item))
			{
				NotifySelectionChanged();
				return true;
			}
			return false;
		}

		public IEnumerator<IStudyItem> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}