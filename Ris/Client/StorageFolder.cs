using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

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

            _itemsTable.Items.ItemsChanged += delegate(object sender, ItemEventArgs args)
                {
                    NotifyTextChanged();
                };
        }

        protected ItemCollection<TItem> Items
        {
            get { return _itemsTable.Items; }
        }

        public override string Text
        {
            get { return string.Format("{0} ({1})", _folderName, this.Items.Count); }
        }

        public override ITable ItemsTable
        {
            get { return _itemsTable; }
        }

        public override void Refresh()
        {
            // do nothing
        }

        public override void RefreshCount()
        {
            // do nothing
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
