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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Associates a folder class with a worklist class.
	/// </summary>
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

	/// <summary>
	/// Abstract base class for workflow folders.  A workflow folder is characterized by the fact
	/// that it contains "work items".
	/// </summary>
	public abstract class WorkflowFolder : Folder, IDisposable
	{
        private int _itemCount = -1;
        private bool _isPopulated;

        private Timer _refreshTimer;
        private int _refreshTime;


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

		#region Public API

		/// <summary>
		/// Gets or sets the auto-refresh interval for this folder.
		/// </summary>
		public int RefreshTime
		{
			get { return _refreshTime; }
			set
			{
				_refreshTime = value;
				this.RestartRefreshTimer();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this folder is populated or not.
		/// </summary>
		public bool IsPopulated
		{
			get { return _isPopulated; }
			protected set { _isPopulated = value; }
		}

		#endregion

		#region Folder overrides

		/// <summary>
		/// Opens the folder (i.e. instructs the folder to show its "open" state icon).
		/// </summary>
		public override void OpenFolder()
        {
            base.OpenFolder();

            this.RestartRefreshTimer();
        }

		/// <summary>
		/// Closes the folder (i.e. instructs the folder to show its "closed" state icon).
		/// </summary>
		public override void CloseFolder()
        {
            base.CloseFolder();

            this.RestartRefreshTimer();
        }

		/// <summary>
		/// Gets the total number of items "contained" in this folder, which may be the same
		/// as the number of items displayed in the <see cref="IFolder.ItemsTable"/>, or may be larger
		/// in the event the table is only showing a subset of the total number of items.
		/// </summary>
		public override int TotalItemCount
        {
            get { return _itemCount; }
        }

		/// <summary>
		/// Gets a value indicating whether the count of items in the folder is currently known.
		/// </summary>
		protected override bool IsItemCountKnown
        {
            get { return _isPopulated || _itemCount > -1; }
		}

		#endregion


		#region Protected API

		/// <summary>
		/// Sets the total "logical" item count for this folder, which may be greater than the number of items in the folder,
		/// for example, if there are too many items to display.
		/// </summary>
		/// <param name="n"></param>
        protected void SetTotalItemCount(int n)
        {
            if (n != _itemCount)
            {
                _itemCount = n;
                NotifyTotalItemCountChanged();
                NotifyTextChanged();
            }
        }

		/// <summary>
		/// Restarts the refresh timer.
		/// </summary>
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

		#endregion
	}

    /// <summary>
    /// Abstract base class for folders that display the contents of worklists.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
	public abstract class WorkflowFolder<TItem> : WorkflowFolder
		where TItem : DataContractBase
    {
        #region QueryItemsResult class

        protected class QueryItemsResult
        {
            private readonly IList<TItem> _items;
            private readonly int _totalItemCount;

            public QueryItemsResult(IList<TItem> items, int totalItemCount)
            {
                _items = items;
                _totalItemCount = totalItemCount;
            }

            public IList<TItem> Items
            {
                get { return _items; }
            }

            public int TotalItemCount
            {
                get { return _totalItemCount; }
            }
        }

        #endregion

        private readonly Table<TItem> _itemsTable;
        private IDropHandler<TItem> _currentDropHandler;

        private BackgroundTask _queryItemsTask;
        private BackgroundTask _queryCountTask;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemsTable"></param>
        public WorkflowFolder(Table<TItem> itemsTable)
        {
            _itemsTable = itemsTable;
            _itemsTable.Items.ItemsChanged += delegate
                {
                    SetTotalItemCount(_itemsTable.Items.Count);
                };
		}

		#region Folder overrides

		/// <summary>
    	/// Gets a table of the items that are contained in this folder
    	/// </summary>
    	public override ITable ItemsTable
        {
            get
            {
                return _itemsTable;
            }
        }

    	/// <summary>
    	/// Asks the folder to refresh its contents.  The implementation may be asynchronous.
    	/// </summary>
    	public override void  Refresh()
        {
            if (_queryItemsTask != null)
            {
                // refresh already in progress
                return;
            }

            _queryItemsTask = new BackgroundTask(
                delegate(IBackgroundTaskContext taskContext)
                {
                    try
                    {
                        QueryItemsResult result = QueryItems();
                        taskContext.Complete(result);
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

    	/// <summary>
    	/// Asks the folder to refresh the count of its contents, without actually refreshing the contents.
    	/// The implementation may be asynchronous.
    	/// </summary>
    	public override void RefreshCount()
		{
			if (_queryCountTask != null)
			{
				// refresh already in progress
				return;
			}

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

    	/// <summary>
    	/// Informs the folder that the specified items were dragged from it.  It is up to the implementation
    	/// of the folder to determine the appropriate response (e.g. whether the items should be removed or not).
    	/// </summary>
    	/// <param name="items"></param>
    	public override void DragComplete(object[] items, DragDropKind kind)
		{
			if (kind == DragDropKind.Move)
			{
				// items have been "moved" out of this folder
			}
		}

    	/// <summary>
    	/// Asks the folder if it can accept a drop of the specified items
    	/// </summary>
    	/// <param name="items"></param>
    	/// <param name="kind"></param>
    	/// <returns></returns>
    	public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
		{
			// this cast is not terribly safe, but in practice it should always succeed
    		WorkflowFolderSystem fs = (WorkflowFolderSystem) this.FolderSystem;
			_currentDropHandler = (IDropHandler<TItem>)fs.GetDropHandler(this, items);

			// if the items are acceptable, return Move (never Copy, which would make no sense for a workflow folder)
			return _currentDropHandler != null ? DragDropKind.Move : DragDropKind.None;
		}

    	/// <summary>
    	/// Instructs the folder to accept the specified items
    	/// </summary>
    	/// <param name="items"></param>
    	/// <param name="kind"></param>
    	public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
		{
			if (_currentDropHandler == null)
				return DragDropKind.None;

			// cast items to type safe collection
			ICollection<TItem> dropItems = CollectionUtils.Map<object, TItem>(items, delegate(object item) { return (TItem)item; });
			return _currentDropHandler.ProcessDrop(dropItems) ? DragDropKind.Move : DragDropKind.None;
		}

		#endregion

		#region Helpers

		private void OnQueryItemsCompleted(object sender, BackgroundTaskTerminatedEventArgs args)
        {
            if(args.Reason == BackgroundTaskTerminatedReason.Completed)
            {
                NotifyRefreshBegin();

                QueryItemsResult result = (QueryItemsResult)args.Result;
                this.IsPopulated = true;
                _itemsTable.Items.Clear();
                _itemsTable.Items.AddRange(result.Items);
                _itemsTable.Sort();

                NotifyRefreshFinish();
            }
            else
            {
				// since this was running in the background, we can't report the exception to the user
				// because they would have no context for it, and it would be annoying
				// therefore, just log it
                Platform.Log(LogLevel.Error, args.Exception);
            }

            // dispose of the task
            _queryItemsTask.Terminated -= OnQueryItemsCompleted;
            _queryItemsTask.Dispose();
            _queryItemsTask = null;

            this.RestartRefreshTimer();
        }

        private void OnQueryCountCompleted(object sender, BackgroundTaskTerminatedEventArgs args)
        {
            if (args.Reason == BackgroundTaskTerminatedReason.Completed)
            {
                SetTotalItemCount((int)args.Result);
            }
            else
            {
				// since this was running in the background, we can't report the exception to the user
				// because they would have no context for it, and it would be annoying
				// therefore, just log it
				Platform.Log(LogLevel.Error, args.Exception);
            }

            // dispose of the task
            _queryCountTask.Terminated -= OnQueryCountCompleted;
            _queryCountTask.Dispose();
            _queryCountTask = null;

            this.RestartRefreshTimer();
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Called to obtain the set of items in the folder.
		/// </summary>
		/// <returns></returns>
		protected abstract QueryItemsResult QueryItems();

		/// <summary>
		/// Called to obtain a count of the logical total number of items in the folder (which may be more than the number in memory).
		/// </summary>
		/// <returns></returns>
    	protected abstract int QueryCount();

		#endregion
	}

}
