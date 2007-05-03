using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    public abstract class WorkflowFolder<TItem> : Folder, IDisposable
    {
        private string _folderName;
        private Table<TItem> _itemsTable;
        private bool _isPopulated;
        private int _itemCount = -1;
        private WorkflowFolderSystem<TItem> _folderSystem;

        private Timer _refreshTimer;
        private int _refreshTime;

        public WorkflowFolder(WorkflowFolderSystem<TItem> folderSystem, string folderName, Table<TItem> itemsTable)
        {
            _folderSystem = folderSystem;
            _folderName = folderName;
            _itemsTable = itemsTable;
            _itemsTable.Items.ItemsChanged += delegate(object sender, ItemEventArgs args)
                {
                    _itemCount = _itemsTable.Items.Count;
                    NotifyTextChanged();
                };
        }

        public void UpdateWorklistItem(TItem item)
        {
            // if the folder has not yet been populated, then nothing to do
            if (!_isPopulated)
                return;

            // get the index of the item in this folder, if it exists
            int i = _itemsTable.Items.FindIndex(delegate(TItem x) { return x.Equals(item); });

            // is the item a member of this folder?
            if (IsMember(item))
            {
                if (i > -1)
                {
                    // update the item that is already contained in this folder
                    _itemsTable.Items[i] = item;
                }
                else
                {
                    // add the item, because it was not already contained in this folder
                    _itemsTable.Items.Add(item);
                }
            }
            else
            {
                if (i > -1)
                {
                    // remove the item from this folder, because it is no longer a member
                    _itemsTable.Items.RemoveAt(i);
                }
            }
        }

        public override string Text
        {
            get
            {
                return _isPopulated || _itemCount >= 0 ?
                    string.Format("{0} ({1})", _folderName, _itemCount) : _folderName;
            }
        }

        public int ItemCount
        {
            get { return _itemCount; }
            set
            {
                _itemCount = value;
                NotifyTextChanged();
            }
        }

        public override ITable ItemsTable
        {
            get
            {
                return _itemsTable;
            }
        }

        public WorkflowFolderSystem<TItem> WorkflowFolderSystem
        {
            get { return _folderSystem; }
        }

        public int RefreshTime
        {
            get { return _refreshTime; }
            set
            {
                _refreshTime = value;
                this.RestartRefreshTimer();
            }
        }

        public override void Refresh()
        {
            try
            {
                if (CanQuery())
                {
                    NotifyRefreshBegin();

                    IList<TItem> items = QueryItems();
                    _isPopulated = true;
                    _itemsTable.Items.Clear();
                    _itemsTable.Items.AddRange(items);
                    _itemsTable.Sort();

                    NotifyRefreshFinish();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, "Folder refresh failed", _folderSystem.DesktopWindow);
            }
        }

        public void RefreshCount()
        {
            try
            {
                if (CanQuery())
                {
                    this.ItemCount = QueryCount();
                }
            }
            catch (Exception e)
            {
                // TODO report this, but don't show messagebox
                Console.WriteLine(e.Message);
            }
        }

        public override void OpenFolder()
        {
            base.OpenFolder();

            this.RestartRefreshTimer();
        }

        public override void CloseFolder()
        {
            base.CloseFolder();

            this.RestartRefreshTimer();
        }

        public override void DragComplete(object[] items, DragDropKind kind)
        {
            if (kind == DragDropKind.Move)
            {
                // items have been "moved" out of this folder
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
                }
                catch (Exception e)
                {
                    // TODO report this
                    Console.WriteLine(e.Message);
                }
            }

            return DragDropKind.Move;
        }

        protected void RestartRefreshTimer()
        {
            if (_refreshTimer != null)
            {
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }

            if (_refreshTime > 0)
            {
                if (this.IsOpen)
                    _refreshTimer = new Timer(new TimerDelegate(Refresh), _refreshTime, _refreshTime);
                else
                    _refreshTimer = new Timer(new TimerDelegate(RefreshCount), 1000, _refreshTime);
            }
        }

        protected abstract bool CanQuery();
        protected abstract int QueryCount();
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


        protected abstract bool IsMember(TItem item);

        #region IDisposable Members

        public void Dispose()
        {
            if (_refreshTimer != null)
            {
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }
        }

        #endregion
    }
}
