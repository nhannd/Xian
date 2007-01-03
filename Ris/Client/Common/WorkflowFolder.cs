using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Common
{
    public abstract class WorkflowFolder<TItem> : Folder
    {
        private string _folderName;
        private Table<TItem> _itemsTable;

        public WorkflowFolder(string folderName, Table<TItem> itemsTable)
        {
            _folderName = folderName;
            _itemsTable = itemsTable;
            _itemsTable.Items.ItemsChanged += delegate(object sender, ItemEventArgs args)
                {
                    NotifyTextChanged();
                };
        }

        public override string Text
        {
            get { return string.Format("{0} ({1})", _folderName, this._itemsTable.Items.Count); }
        }

        public override ITable ItemsTable
        {
            get
            {
                return _itemsTable;
            }
        }

        public override void Refresh()
        {
            IList<TItem> items = QueryItems();
            _itemsTable.Items.Clear();
            _itemsTable.Items.AddRange(items);
        }

        public override void DragComplete(object[] items, DragDropKind kind)
        {
            if (kind == DragDropKind.Move)
            {
                // items have been "moved" out of this folder
                // however, we don't know, in general, whether this folder still "contains" them or not
                // so we need a refresh
                Refresh();
            }
        }

        public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
        {
            bool acceptable = CollectionUtils.TrueForAll(items,
                delegate(object item)
                {
                    return (item is TItem) && CanAcceptDrop((TItem)item);
                });

            // if the items are acceptable, return Move (never Copy, which would make no sense for a workflow folder)
            return acceptable ? DragDropKind.Move : DragDropKind.None;
        }

        public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
        {
            if (!ConfirmAcceptDrop(CollectionUtils.Map<object, TItem>(items, delegate(object item) { return (TItem)item; })))
                return DragDropKind.None;

            foreach (TItem item in items)
            {
                try
                {
                    bool processed = ProcessDrop(item);
                    if(processed)
                        _itemsTable.Items.Add(item);
                }
                catch (Exception e)
                {
                    // TODO report this
                }
            }

            return DragDropKind.Move;
        }

        protected abstract IList<TItem> QueryItems();
        protected abstract bool CanAcceptDrop(TItem item);
        protected abstract bool ConfirmAcceptDrop(ICollection<TItem> items);

        /// <summary>
        /// Subclass implements this to process a dropped item.  Return true if the item was actually added to this folder, otherwise false.
        /// Throw an exception to indicate an unexpected condition.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract bool ProcessDrop(TItem item);

    }
}
