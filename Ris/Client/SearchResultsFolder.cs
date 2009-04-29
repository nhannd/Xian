using System;
using System.Collections.Generic;
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
        private SearchParams _searchParams;
    	private bool _isValid;

        protected SearchResultsFolder()
        {
        }

        /// <summary>
        /// Gets or sets the search arguments.  Setting this property will automatically
        /// call <see cref="Folder.Invalidate"/> on this folder.
        /// </summary>
        public SearchParams SearchParams
        {
            get { return _searchParams; }
            set
            {
                _searchParams = value;

				// invalidate the folder immediately
            	Invalidate();
            }
        }

        #region Folder overrides

		protected override void InvalidateCore()
		{
			_isValid = false;
		}

		protected override bool UpdateCore()
		{
			// only initiate a query if this folder is open (eg selected)
			// this folder does not support updating the count independently of the items
			if(this.IsOpen && !_isValid)
			{
				BeginQueryItems();
				_isValid = true;
				return true;
			}
			return false;
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
		/// <param name="itemsTable"></param>
		protected SearchResultsFolder(Table<TItem> itemsTable)
		{
			_itemsTable = itemsTable;
		}

		#region Folder overrides

		public override ITable ItemsTable
		{
			get { return _itemsTable; }
		}

		protected override void BeginQueryCount()
		{
			// not supported on search folders
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Called to execute the search query.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="specificityThreshold"></param>
		/// <returns></returns>
		protected abstract TextQueryResponse<TItem> DoQuery(SearchParams query, int specificityThreshold);

		#endregion

		#region Helpers

		protected override void BeginQueryItems()
		{
			if (_queryItemsTask != null)
			{
				// a search is already in progress, and should be abandoned
				// unsubscribe from its Terminated event, which effectively orphans it
				_queryItemsTask.Terminated -= OnQueryItemsCompleted;
			}

			if (this.SearchParams != null)
			{
				BeginUpdate();

				_queryItemsTask = new BackgroundTask(
					delegate(IBackgroundTaskContext taskContext)
					{
						try
						{
							TextQueryResponse<TItem> response = DoQuery(this.SearchParams, SearchCriteriaSpecificityThreshold);
							if (response.TooManyMatches)
								throw new WeakSearchCriteriaException();
							taskContext.Complete(response.Matches ?? new List<TItem>());
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
			if (args.Reason == BackgroundTaskTerminatedReason.Completed)
			{
				NotifyItemsTableChanging();

				IList<TItem> items = (IList<TItem>)args.Result;
				this.TotalItemCount = items.Count;
				_itemsTable.Items.Clear();
				_itemsTable.Items.AddRange(items);
				_itemsTable.Sort();

				NotifyItemsTableChanged();
			}
			else
			{
				ExceptionHandler.Report(args.Exception, this.FolderSystem.DesktopWindow);
			}

			// dispose of the task
			_queryItemsTask.Terminated -= OnQueryItemsCompleted;
			_queryItemsTask.Dispose();
			_queryItemsTask = null;

			EndUpdate();
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

        protected override TextQueryResponse<TItem> DoQuery(SearchParams query, int specificityThreshold)
        {
			WorklistItemTextQueryOptions options = WorklistItemTextQueryOptions.PatientOrder
				| (DowntimeRecovery.InDowntimeRecoveryMode ? WorklistItemTextQueryOptions.DowntimeRecovery : 0);

			return DoQueryCore(query, specificityThreshold, options, this.ProcedureStepClassName);
        }

		protected static TextQueryResponse<TItem> DoQueryCore(SearchParams query, int specificityThreshold, WorklistItemTextQueryOptions options, string procedureStepClassName)
        {
			TextQueryResponse<TItem> response = null;

			WorklistItemTextQueryRequest request = new WorklistItemTextQueryRequest(
						query.TextSearch, specificityThreshold, procedureStepClassName, options);

			if (query.UseAdvancedSearch)
			{
				request.UseAdvancedSearch = query.UseAdvancedSearch;
				request.SearchFields = query.SearchFields;
			}

			Platform.GetService<TWorklistService>(
				delegate(TWorklistService service)
				{
					response = service.SearchWorklists(request);
				});

			return response;
		}

        protected abstract string ProcedureStepClassName { get; }
    }
}
