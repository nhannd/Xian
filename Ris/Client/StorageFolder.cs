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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    public class StorageFolder<TItem> : Folder
    {
        private string _folderName;

        private Table<TItem> _itemsTable;

        public StorageFolder(string folderName, Table<TItem> itemsTable)
        {
            _folderName = folderName;
            _itemsTable = itemsTable;

            _itemsTable.Items.ItemsChanged += delegate
                {
                	this.TotalItemCount = _itemsTable.Items.Count;
                };
        }

        protected ItemCollection<TItem> Items
        {
            get { return _itemsTable.Items; }
        }

		protected override bool IsItemCountKnown
		{
			get { return true; }
		}

		protected override bool UpdateCore()
		{
			// do nothing
			return false;
		}

		protected override void InvalidateCore()
		{
			// do nothing
		}

		public override ITable ItemsTable
        {
            get { return _itemsTable; }
        }

        public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
        {
            // return the requested kind if all items are of type TItem, otherwise None
            return CollectionUtils.TrueForAll(items, delegate(object obj) { return obj is TItem; }) ? kind : DragDropKind.None;
        }

        public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
        {
            if (kind != DragDropKind.None)
            {
                // store any items that are not already in this folder
                foreach (TItem item in items)
                {
                    if (!CollectionUtils.Contains<TItem>(this.Items, delegate(TItem x) { return x.Equals(item); }))
                    {
                        this.Items.Add(item);
                    }
                }
            }

            return kind;
        }

        public override void DragComplete(object[] items, DragDropKind kind)
        {
            // if the operation was a Move, then we should remove the items from this folder
            if (kind == DragDropKind.Move)
            {
                foreach (TItem item in items)
                {
                    this.Items.Remove(item);
                }
            }
        }
    }
}
