using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Base class for folders that display search results.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public abstract class SearchResultsFolder<TItem> : Folder
		where TItem : DataContractBase
	{
		private readonly WorkflowFolderSystem<TItem> _folderSystem;
		private readonly Table<TItem> _itemsTable;

		private BackgroundTask _queryItemsTask;
		private SearchData _searchData;
		private bool _isPopulated;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="folderSystem"></param>
		/// <param name="itemsTable"></param>
		protected SearchResultsFolder(WorkflowFolderSystem<TItem> folderSystem, Table<TItem> itemsTable)
		{
			_folderSystem = folderSystem;
			_itemsTable = itemsTable;

			// no need to refresh this folder every time it is opened
			this.RefreshOnOpen = false;
		}

		/// <summary>
		/// Gets or sets the search arguments.  Setting this property will automatically
		/// initiate a query to refresh the contents of this folder.
		/// </summary>
		public SearchData SearchData
		{
			get { return _searchData; }
			set
			{
				_searchData = value;
				this.Refresh();
			}
		}

		#region Folder overrides

		protected override bool IsItemCountKnown
		{
			get { return _isPopulated; }
		}

		public override void RefreshCount()
		{
			// do nothing
		}

		public override ITable ItemsTable
		{
			get { return _itemsTable; }
		}

		public override int TotalItemCount
		{
			get { return _itemsTable.Items.Count; }
		}

		public override void Refresh()
		{
			if (_queryItemsTask != null)
			{
				// refresh already in progress
				return;
			}

			if (_searchData != null)
			{
				_queryItemsTask = new BackgroundTask(
					delegate(IBackgroundTaskContext taskContext)
					{
						try
						{
							IList<TItem> result = QueryHelper();
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
		}

		protected override IconSet OpenIconSet
		{
			get
			{
				return new IconSet(IconScheme.Colour, "SearchFolderOpenSmall.png", "SearchFolderOpenMedium.png", "SearchFolderOpenLarge.png");
			}
		}

		protected override IconSet ClosedIconSet
		{
			get
			{
				return new IconSet(IconScheme.Colour, "SearchFolderClosedSmall.png", "SearchFolderClosedMedium.png", "SearchFolderClosedLarge.png");
			}
		}

		#endregion

		#region Overridables

		/// <summary>
		/// Called to execute the search query.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected abstract TextQueryResponse<TItem> DoQuery(TextQueryRequest request);

		#endregion

		#region Helpers

		protected int SearchCriteriaSpecificityThreshold
		{
			get { return HomePageSettings.Default.SearchCriteriaSpecificityThreshold; }
		}

		private IList<TItem> QueryHelper()
		{
			List<TItem> worklistItems = null;
			TextQueryRequest request = new TextQueryRequest();
			request.TextQuery = this.SearchData.TextSearch;
			request.SpecificityThreshold = this.SearchCriteriaSpecificityThreshold;

			TextQueryResponse<TItem> response = DoQuery(request);
			if (response.TooManyMatches)
				throw new WeakSearchCriteriaException();
			worklistItems = response.Matches;
			
			if (worklistItems == null)
				worklistItems = new List<TItem>();

			return worklistItems;
		}

		private void OnQueryItemsCompleted(object sender, BackgroundTaskTerminatedEventArgs args)
		{
			if (args.Reason == BackgroundTaskTerminatedReason.Completed)
			{
				NotifyRefreshBegin();

				IList<TItem> items = (IList<TItem>)args.Result;
				_isPopulated = true;
				_itemsTable.Items.Clear();
				_itemsTable.Items.AddRange(items);
				_itemsTable.Sort();

				NotifyRefreshFinish();
				NotifyTotalItemCountChanged();
				NotifyTextChanged();
			}
			else
			{
				ExceptionHandler.Report(args.Exception, _folderSystem.DesktopWindow);
			}

			// dispose of the task
			_queryItemsTask.Terminated -= OnQueryItemsCompleted;
			_queryItemsTask.Dispose();
			_queryItemsTask = null;
		}

		#endregion
	}
}
