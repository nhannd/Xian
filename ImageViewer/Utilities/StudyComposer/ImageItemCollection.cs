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
using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	public sealed class ImageItemCollection : BindingList<ImageItem>
	{
		// mappings between nodes and items
		private readonly Dictionary<SopInstanceNode, ImageItem> _map = new Dictionary<SopInstanceNode, ImageItem>();

		// reference to the underlying list
		private readonly SopInstanceNodeCollection _list;

		internal ImageItemCollection(SopInstanceNodeCollection list)
		{
			_list = list;
			for (int n = 0; n < list.Count; n++)
			{
				//TODO: implement some way to render an icon from the dicom attributes instead of needing an ipresentationimage
				//so that we can stick the same UI mode on top of a studybuilder session created programmatically
				//so when we implement that, we can uncomment this line below and allow preinitialization of the collection
				//base.Insert(n, new ImageItem(list[n], null));
			}
		}

		internal ImageItem GetByUid(DicomFile dicomFile, IPresentationImage image)
		{
			SopInstanceNode node = _list.GetImageByUid(dicomFile);
			if (!_map.ContainsKey(node))
				base.Insert(_list.IndexOf(node), new ImageItem(node, image));
			return _map[node];
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			int id = base.IndexOf(sender as ImageItem);
			if (id >= 0)
				base.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, id));
		}

		protected override object AddNewCore()
		{
			//TODO: see notes in constructor (and this is why we disable Add New functionality at the image level)
			//return new ImageItem(new SopInstanceNode(), null);
			throw new NotSupportedException();
		}

		protected override void ClearItems()
		{
			foreach (ImageItem item in _map.Values)
			{
				item.PropertyChanged -= Item_PropertyChanged;
			}
			base.ClearItems();
			_map.Clear();
			_list.Clear();
		}

		protected override void InsertItem(int index, ImageItem item)
		{
			SopInstanceNode node = item.Node;
			_map.Add(node, item);
			if (!_list.Contains(node)) // this method is also called when initializing the list from the list, so we need to check this to avoid re-adding
				_list.Insert(index, node);

			base.InsertItem(index, item);

			item.PropertyChanged += Item_PropertyChanged;
		}

		protected override void RemoveItem(int index)
		{
			SopInstanceNode node = base[index].Node;

			_map[node].PropertyChanged -= Item_PropertyChanged;
			_map.Remove(node);
			_list.Remove(node);

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, ImageItem item)
		{
			SopInstanceNode oldNode = base[index].Node;
			SopInstanceNode newNode = item.Node;
			_map.Add(newNode, item);
			_map[oldNode].PropertyChanged -= Item_PropertyChanged;
			_map.Remove(oldNode);
			_list.Remove(oldNode);
			_list.Insert(index, newNode);
			item.PropertyChanged += Item_PropertyChanged;

			base.SetItem(index, item);
		}
	}
}