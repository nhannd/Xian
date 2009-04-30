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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using System.Threading;

namespace ClearCanvas.Ris.Client
{
	#region FolderForWorklistClassAttribute

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

	#endregion


	/// <summary>
	/// Abstract base class for workflow folders.  A workflow folder is characterized by the fact
	/// that it contains "work items".
	/// </summary>
	public abstract class WorkflowFolder : Folder
	{
		private bool _isCountValid;
		private bool _isItemsValid;

		private static readonly IconSet _closedRefreshingIconSet = new IconSet("FolderClosedRefreshMedium.gif");
		private static readonly IconSet _openRefreshingIconSet = new IconSet("FolderOpenRefreshMedium.gif");



		#region Folder overrides

		protected override IconSet ClosedIconSet
		{
			get { return IsUpdating ? _closedRefreshingIconSet : base.ClosedIconSet; }
		}

		protected override IconSet OpenIconSet
		{
			get { return IsUpdating ? _openRefreshingIconSet : base.OpenIconSet; }
		}

		protected override void InvalidateCore()
		{
			_isCountValid = false;
			_isItemsValid = false;
		}

		protected override bool UpdateCore()
		{
			if(this.IsOpen)
			{
				// if the folder is open, the actual contents need to be valid
				if (!_isItemsValid)
				{
					// an items query validates the count as well as the items
					BeginQueryItems();
					_isItemsValid = true;
					_isCountValid = true;
					return true;
				}
			}
			else
			{
				// otherwise, only the count needs to be valid
				if (!_isCountValid)
				{
					BeginQueryCount();
					_isCountValid = true;
					return true;
				}
			}

			return false;	// nothing updated
		}

		#endregion


		#region Protected API

		/// <summary>
		/// 
		/// </summary>
		protected abstract void BeginQueryItems();

		protected abstract void BeginQueryCount();

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

		/// <summary>
		/// Asks the folder to refresh its contents.  The implementation may be asynchronous.
		/// </summary>
		protected override void BeginQueryItems()
		{
			if (_queryItemsTask != null)
			{
				// refresh already in progress
				return;
			}

			// notify the framework that an update is beginning
			BeginUpdate();

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

			NotifyIconChanged();
		}

		/// <summary>
		/// Asks the folder to refresh the count of its contents, without actually refreshing the contents.
		/// The implementation may be asynchronous.
		/// </summary>
		protected override void BeginQueryCount()
		{
			if (_queryCountTask != null)
			{
				// refresh already in progress
				return;
			}

			// notify the framework that an update is beginning
			BeginUpdate();

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

			NotifyIconChanged();
		}

		private void OnQueryItemsCompleted(object sender, BackgroundTaskTerminatedEventArgs args)
        {
            if(args.Reason == BackgroundTaskTerminatedReason.Completed)
            {
                NotifyItemsTableChanging();

                QueryItemsResult result = (QueryItemsResult)args.Result;
            	this.TotalItemCount = result.TotalItemCount;
                _itemsTable.Items.Clear();
                _itemsTable.Items.AddRange(result.Items);
                _itemsTable.Sort();
				InErrorState = false;

                NotifyItemsTableChanged();
            }
            else
            {
				// since this was running in the background, we can't report the exception to the user
				// because they would have no context for it, and it would be annoying
				// therefore, just log it
				InErrorState = true;
                Platform.Log(LogLevel.Error, args.Exception);
            }

            // dispose of the task
            _queryItemsTask.Terminated -= OnQueryItemsCompleted;
            _queryItemsTask.Dispose();
            _queryItemsTask = null;

			EndUpdate();
			NotifyIconChanged();
		}

        private void OnQueryCountCompleted(object sender, BackgroundTaskTerminatedEventArgs args)
        {
            if (args.Reason == BackgroundTaskTerminatedReason.Completed)
            {
				InErrorState = false;
                this.TotalItemCount = (int)args.Result;
            }
            else
            {
				// since this was running in the background, we can't report the exception to the user
				// because they would have no context for it, and it would be annoying
				// therefore, just log it
				InErrorState = true;
				Platform.Log(LogLevel.Error, args.Exception);
            }

            // dispose of the task
            _queryCountTask.Terminated -= OnQueryCountCompleted;
            _queryCountTask.Dispose();
            _queryCountTask = null;

			EndUpdate();
			NotifyIconChanged();
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
