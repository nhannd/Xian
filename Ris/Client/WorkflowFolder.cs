#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FolderForWorklistClassAttribute : Attribute
    {
        private readonly string _worklistClassName;

        public FolderForWorklistClassAttribute(string worklistClassName)
        {
            _worklistClassName = worklistClassName;
        }

        public string WorklistClassName
        {
            get { return _worklistClassName; }
        }
    }

    public abstract class WorkflowFolder<TItem> : Folder, IDisposable
    {
        private readonly string _folderTooltip;
        private readonly Table<TItem> _itemsTable;
        private bool _isPopulated;
        private int _itemCount = -1;
        private readonly WorkflowFolderSystem<TItem> _folderSystem;
        private readonly string _worklistClassName;

        private Timer _refreshTimer;
        private int _refreshTime;

        private ExtensionPoint<IDropHandler<TItem>> _dropHandlerExtensionPoint;
        private IDropContext _dropContext;
        private IDropHandler<TItem> _currentDropHandler;

        private BackgroundTask _queryItemsTask;
        private BackgroundTask _queryCountTask;

        public WorkflowFolder(WorkflowFolderSystem<TItem> folderSystem, string folderName, string folderTooltip, Table<TItem> itemsTable)
        {
            _folderSystem = folderSystem;
            _folderTooltip = folderTooltip;
            _itemsTable = itemsTable;
            _itemsTable.Items.ItemsChanged += delegate
                {
                    _itemCount = _itemsTable.Items.Count;
                    NotifyTextChanged();
                };

            Platform.CheckForNullReference(_folderPath, "FolderPath attribute");

            if (!string.IsNullOrEmpty(folderName))
                _folderPath = new Path(string.Concat(_folderPath.ToString(), "/", folderName), _resourceResolver);

            // Initialize worklist type
            FolderForWorklistClassAttribute attrib = (FolderForWorklistClassAttribute) CollectionUtils.SelectFirst(
                this.GetType().GetCustomAttributes(false),
                delegate(object o)
                    {
                        return o is FolderForWorklistClassAttribute;
                    });

            if (attrib != null)
                _worklistClassName = attrib.WorklistClassName;
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

        public override string Id
        {
            get
            {
                return this.IsStatic
                           ? string.Concat(this.GetType().Name)
                           : string.Concat(this.GetType().Name, ":", this.FolderPath.LastSegment.LocalizedText);
            }
        }

        public string WorklistClassName
        {
            get { return _worklistClassName; }
        }

        public override string Text
        {
            get
            {
                string folderName = _folderPath.LastSegment.LocalizedText;
                return _isPopulated || _itemCount >= 0 ?
                    string.Format("{0} ({1})", folderName, _itemCount) : folderName;
            }
        }

        public override string Tooltip
        {
            get
            {
                return _folderTooltip;
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

        public override void  Refresh()
        {
            if (_queryItemsTask != null)
            {
                // refresh already in progress
                return;
            }

            if (CanQuery())
            {
                _queryItemsTask = new BackgroundTask(
                    delegate(IBackgroundTaskContext taskContext)
                    {
                        try
                        {
                            IList<TItem> items = QueryItems();
                            taskContext.Complete(items);
                        }
                        catch (Exception e)
                        {
                            taskContext.Error(e);
                        }
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
                NotifyRefreshBegin();

                IList<TItem> items = (IList<TItem>)args.Result;
                _isPopulated = true;
                _itemsTable.Items.Clear();
                _itemsTable.Items.AddRange(items);
                _itemsTable.Sort();

                NotifyRefreshFinish();
            }
            else
            {
                // special case: if this is a search folder, the query may have returned to many results
                // this message must be reported to the user
                if (args.Exception is WeakSearchCriteriaException)
                {
                    ExceptionHandler.Report(args.Exception, _folderSystem.DesktopWindow);
                }
                else
                {
                    // otherwise just log the exception
                    Platform.Log(LogLevel.Error, args.Exception);
                }
            }

            // dispose of the task
            _queryItemsTask.Terminated -= OnQueryItemsCompleted;
            _queryItemsTask.Dispose();
            _queryItemsTask = null;

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
                        try
                        {
                            int count = QueryCount();
                            taskContext.Complete(count);
                        }
                        catch (Exception e)
                        {
                            taskContext.Error(e);
                        }
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
                Platform.Log(LogLevel.Error, args.Exception);
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
                _refreshTimer.Stop();
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }

            if (_refreshTime > 0)
            {
                TimerDelegate timerDelegate = this.IsOpen
                    ? new TimerDelegate(delegate(object state) { Refresh(); })
                    : new TimerDelegate(delegate(object state) { RefreshCount(); });

                _refreshTimer = new Timer(timerDelegate);
                _refreshTimer.IntervalMilliseconds = _refreshTime;
                _refreshTimer.Start();
            }
        }

        protected void InitDragDropHandling(ExtensionPoint<IDropHandler<TItem>> dropHandlerExtensionPoint, IDropContext dropContext)
        {
            _dropHandlerExtensionPoint = dropHandlerExtensionPoint;
            _dropContext = dropContext;
        }

        protected int SearchCriteriaSpecificityThreshold
        {
            get { return HomePageSettings.Default.SearchCriteriaSpecificityThreshold; }
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
