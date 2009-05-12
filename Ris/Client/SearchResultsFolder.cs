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
				this.Tooltip = _searchParams.ToString();

				// invalidate the folder immediately
				_isValid = false;
			}
		}

		#region Folder overrides

		protected override void InvalidateCore()
		{
			// do nothing.  Search results only become invalid if SearchParams have changed, not if the folder system 
			// is invalidated.
			return;
		}

		protected override bool UpdateCore()
		{
			// only initiate a query if this folder is open (eg selected)
			// this folder does not support updating the count independently of the items
			if (this.IsOpen && !_isValid)
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
			: base(itemsTable)
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
