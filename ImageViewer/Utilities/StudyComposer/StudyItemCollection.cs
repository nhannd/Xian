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

using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	public sealed class StudyItemCollection : BindingList<StudyItem>
	{
		// mappings between nodes and items
		private readonly Dictionary<StudyNode, StudyItem> _map = new Dictionary<StudyNode, StudyItem>();

		// reference to the underlying collection
		private readonly StudyNodeCollection _collection;

		internal StudyItemCollection(StudyNodeCollection collection)
		{
			_collection = collection;
			foreach (StudyNode node in collection)
			{
				base.Add(new StudyItem(node));
			}
		}

		internal StudyItem GetByUid(DicomAttributeCollection dataSet)
		{
			StudyNode node = _collection.GetStudyByUid(dataSet);
			if (!_map.ContainsKey(node))
				base.Add(new StudyItem(node));
			return _map[node];
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			int id = base.IndexOf(sender as StudyItem);
			if (id >= 0)
				base.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, id));
		}

		protected override object AddNewCore()
		{
			return new StudyItem(new StudyNode());
		}

		protected override void ClearItems()
		{
			foreach (StudyItem item in _map.Values)
			{
				item.PropertyChanged -= Item_PropertyChanged;
			}
			base.ClearItems();
			_map.Clear();
			_collection.Clear();
		}

		protected override void InsertItem(int index, StudyItem item)
		{
			StudyNode node = item.Node;
			_map.Add(node, item);
			if (!_collection.Contains(node)) // this method is also called when initializing the list from the collection, so we need to check this to avoid re-adding
				_collection.Add(node);

			base.InsertItem(index, item);

			item.PropertyChanged += Item_PropertyChanged;
		}

		protected override void RemoveItem(int index)
		{
			StudyNode node = base[index].Node;

			_map[node].PropertyChanged -= Item_PropertyChanged;
			_map.Remove(node);
			_collection.Remove(node);

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, StudyItem item)
		{
			StudyNode oldNode = base[index].Node;
			StudyNode newNode = item.Node;
			_map.Add(newNode, item);
			_map[oldNode].PropertyChanged -= Item_PropertyChanged;
			_map.Remove(oldNode);
			_collection.Remove(oldNode);
			_collection.Add(newNode);
			item.PropertyChanged += Item_PropertyChanged;

			base.SetItem(index, item);
		}
	}
}