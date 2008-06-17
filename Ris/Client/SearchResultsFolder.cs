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
    public abstract class SearchResultsFolder : WorkflowFolder
    {
        private SearchData _searchData;

        protected SearchResultsFolder()
        {
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
            get { return this.IsPopulated; }
        }

        public override void RefreshCount()
        {
            // do nothing
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

        protected static int SearchCriteriaSpecificityThreshold
        {
            get { return HomePageSettings.Default.SearchCriteriaSpecificityThreshold; }
        }

    }


	/// <summary>
	/// Base class for folders that display search results.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
    public abstract class SearchResultsFolder<TItem> : SearchResultsFolder
		where TItem : DataContractBase
	{
		private readonly Table<TItem> _itemsTable;
		private BackgroundTask _queryItemsTask;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="folderSystem"></param>
		/// <param name="itemsTable"></param>
		protected SearchResultsFolder(Table<TItem> itemsTable)
		{
			_itemsTable = itemsTable;
			_itemsTable.Items.ItemsChanged += delegate
				{
					NotifyTotalItemCountChanged();
					NotifyTextChanged();
				};
		}


		#region Folder overrides

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

			if (this.SearchData != null)
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

		#endregion

		#region Overridables

		/// <summary>
		/// Called to execute the search query.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected abstract TextQueryResponse<TItem> DoQuery(string query, int specificityThreshold);

		#endregion

		#region Helpers

		private IList<TItem> QueryHelper()
		{
			List<TItem> worklistItems;
			TextQueryResponse<TItem> response = DoQuery(this.SearchData.TextSearch, SearchCriteriaSpecificityThreshold);
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
				this.IsPopulated = true;
				_itemsTable.Items.Clear();
				_itemsTable.Items.AddRange(items);
				_itemsTable.Sort();

				NotifyRefreshFinish();
			}
			else
			{
				ExceptionHandler.Report(args.Exception, this.FolderSystem.DesktopWindow);
			}

			// dispose of the task
			_queryItemsTask.Terminated -= OnQueryItemsCompleted;
			_queryItemsTask.Dispose();
			_queryItemsTask = null;
		}

		#endregion
	}

    public abstract class WorklistSearchResultsFolder<TItem, TWorklistService> : SearchResultsFolder<TItem>
        where TItem : DataContractBase
        where TWorklistService : IWorklistService<TItem>
    {
        protected WorklistSearchResultsFolder(Table<TItem> itemsTable)
            :base(itemsTable)
		{
        }

        protected override TextQueryResponse<TItem> DoQuery(string query, int specificityThreshold)
        {
            TextQueryResponse<TItem> response = null;
            Platform.GetService<TWorklistService>(
                delegate(TWorklistService service)
                {
                    response = service.SearchWorklists(new WorklistTextQueryRequest(query, specificityThreshold, ProcedureStepClassName));
                });
            return response;
        }

        protected abstract string ProcedureStepClassName { get; }
    }
}
