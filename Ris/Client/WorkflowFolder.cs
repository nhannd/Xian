using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

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

        private ExtensionPoint<IDropHandler<TItem>> _dropHandlerExtensionPoint;
        private IDropContext _dropContext;
        private IDropHandler<TItem> _currentDropHandler;

        private BackgroundTask _queryItemsTask;
        private BackgroundTask _queryCountTask;

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
            if (_queryItemsTask != null)
            {
                // refresh already in progress
                return;
            }

            if (CanQuery())
            {
                NotifyRefreshBegin();

                _queryItemsTask = new BackgroundTask(
                    delegate(IBackgroundTaskContext taskContext)
                    {
                        IList<TItem> items = QueryItems();
                        taskContext.Complete(items);
                    },
                    false);

                _queryItemsTask.Terminated += OnQueryItemsCompleted;
                _queryItemsTask.Run();
            }
        }

        private void OnQueryItemsCompleted(object sender, BackgroundTaskTerminatedEventArgs args)
        {
            if(args.Reason == BackgroundTaskTerminatedReason.Completed)
            {
                IList<TItem> items = (IList<TItem>)args.Result;
                _isPopulated = true;
                _itemsTable.Items.Clear();
                _itemsTable.Items.AddRange(items);
                _itemsTable.Sort();
            }
            else
            {
                ExceptionHandler.Report(args.Exception, "Folder refresh failed", _folderSystem.DesktopWindow);
            }

            // dispose of the task
            _queryItemsTask.Terminated -= OnQueryItemsCompleted;
            _queryItemsTask.Dispose();
            _queryItemsTask = null;

            NotifyRefreshFinish();

            this.RestartRefreshTimer();
        }

        public override void RefreshCount()
        {
            if (_queryCountTask != null)
            {
                // refresh already in progress
                return;
            }

            if (CanQuery())
            {
                _queryCountTask = new BackgroundTask(
                    delegate(IBackgroundTaskContext taskContext)
                    {
                        int count = QueryCount();
                        taskContext.Complete(count);
                    },
                    false);

                _queryCountTask.Terminated += OnQueryCountCompleted;
                _queryCountTask.Run();

            }
        }

        private void OnQueryCountCompleted(object sender, BackgroundTaskTerminatedEventArgs args)
        {
            if (args.Reason == BackgroundTaskTerminatedReason.Completed)
            {
                this.ItemCount = (int)args.Result;
            }
            else
            {
                ExceptionHandler.Report(args.Exception, "Folder refresh failed", _folderSystem.DesktopWindow);
            }

            // dispose of the task
            _queryCountTask.Terminated -= OnQueryCountCompleted;
            _queryCountTask.Dispose();
            _queryCountTask = null;

            this.RestartRefreshTimer();
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
            if (_dropHandlerExtensionPoint == null)
                return DragDropKind.None;

            // cast items to type safe collection
            ICollection<TItem> dropItems = CollectionUtils.Map<object, TItem>(items, delegate(object item) { return (TItem)item; });

            // check for a handler that can accept
            _currentDropHandler = CollectionUtils.SelectFirst<IDropHandler<TItem>>(_dropHandlerExtensionPoint.CreateExtensions(),
                delegate(IDropHandler<TItem> handler)
                {
                    return handler.CanAcceptDrop(_dropContext, dropItems);
                });

            // if the items are acceptable, return Move (never Copy, which would make no sense for a workflow folder)
            return _currentDropHandler != null ? DragDropKind.Move : DragDropKind.None;
        }

        public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
        {
            if (_currentDropHandler == null)
                return DragDropKind.None;

            // cast items to type safe collection
            ICollection<TItem> dropItems = CollectionUtils.Map<object, TItem>(items, delegate(object item) { return (TItem)item; });
            return _currentDropHandler.ProcessDrop(_dropContext, dropItems) ? DragDropKind.Move : DragDropKind.None;
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
                    _refreshTimer = new Timer(new TimerDelegate(RefreshCount), _refreshTime, _refreshTime);
            }
        }

        protected void InitDragDropHandling(ExtensionPoint<IDropHandler<TItem>> dropHandlerExtensionPoint, IDropContext dropContext)
        {
            _dropHandlerExtensionPoint = dropHandlerExtensionPoint;
            _dropContext = dropContext;
        }

        protected abstract bool CanQuery();
        protected abstract int QueryCount();
        protected abstract IList<TItem> QueryItems();
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
