#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint]
	public sealed class StudyBrowserToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public sealed class StudyBrowserComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public interface IStudyBrowserToolContext : IToolContext
	{
		StudyItem SelectedStudy { get; }

		ReadOnlyCollection<StudyItem> SelectedStudies { get; }

		AEServerGroup SelectedServerGroup { get; }

		event EventHandler SelectedStudyChanged;

		event EventHandler SelectedServerChanged;

		ClickHandlerDelegate DefaultActionHandler { get; set; }

		IDesktopWindow DesktopWindow { get; }

		void RefreshStudyTable();
	}

	[AssociateView(typeof(StudyBrowserComponentViewExtensionPoint))]
	public partial class StudyBrowserComponent : ApplicationComponent, IStudyBrowserComponent
	{
		#region Tool Context

		private class StudyBrowserToolContext : ToolContext, IStudyBrowserToolContext
		{
			private readonly StudyBrowserComponent _component;

			public StudyBrowserToolContext(StudyBrowserComponent component)
			{
				Platform.CheckForNullReference(component, "component");
				_component = component;
			}

			#region IStudyBrowserToolContext Members

			public StudyItem SelectedStudy
			{
				get
				{
					return _component.SelectedStudy;
				}
			}

			public ReadOnlyCollection<StudyItem> SelectedStudies
			{
				get
				{
					return _component.SelectedStudies;
				}
			}

			public AEServerGroup SelectedServerGroup
			{
				get { return _component._selectedServerGroup; }
			}

			public event EventHandler SelectedStudyChanged
			{
				add { _component.SelectedStudyChanged += value; }
				remove { _component.SelectedStudyChanged -= value; }
			}

			public event EventHandler SelectedServerChanged
			{
				add { _component.SelectedServerChanged += value; }
				remove { _component.SelectedServerChanged -= value; }
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _component._defaultActionHandler; }
				set { _component._defaultActionHandler = value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public void RefreshStudyTable()
			{
				try
				{
					_component.Search(_component._lastQueryParametersList);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, _component.Host.DesktopWindow);
				}
			}

			#endregion
		}

		#endregion

		#region Fields

		private List<QueryParameters> _lastQueryParametersList;
		private readonly Dictionary<string, SearchResult> _searchResults;
	    private SearchResult _currentSearchResult;

		private readonly Table<StudyItem> _dummyStudyTable;
		private event EventHandler _studyTableChanged;
		private bool _filterDuplicateStudies = true;

		private ISelection _currentSelection;
		private event EventHandler _selectedStudyChangedEvent;

		private AEServerGroup _selectedServerGroup = new AEServerGroup();
		private event EventHandler _selectedServerChangedEvent;

		private ToolSet _toolSet;
		private ClickHandlerDelegate _defaultActionHandler;

		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;

		private SearchResultColumnOptionCollection _searchResultColumnOptions;

		private bool _isEnabled = true;
		private bool _searchInProgress;

	    #endregion

		public StudyBrowserComponent()
		{
			_dummyStudyTable = new Table<StudyItem>();
			_searchResults = new Dictionary<string, SearchResult>();

            var queryParams = new QueryParameters();
			_lastQueryParametersList = new List<QueryParameters> { queryParams };
		}

		#region Properties/Events

		public AEServerGroup SelectedServerGroup
		{
			get { return _selectedServerGroup; }
			set
			{
                if (ReferenceEquals(value, _selectedServerGroup))
                    return;

                _selectedServerGroup = value;

			    SearchResult searchResult;
				if (!_searchResults.ContainsKey(_selectedServerGroup.GroupID))
				{
					searchResult = new SearchResult();
					searchResult.Initialize();
					_searchResults.Add(_selectedServerGroup.GroupID, searchResult);
				}
                else
				{
				    searchResult = _searchResults[_selectedServerGroup.GroupID];
				}

			    CurrentSearchResult = searchResult;

			    if (_searchResultColumnOptions != null)
                    _searchResultColumnOptions.ApplyColumnSettings(searchResult);
				
                OnSelectedServerChanged();
			}
		}

		internal bool FilterDuplicateStudies
		{
			get { return _filterDuplicateStudies; }
			set
			{
				if (_filterDuplicateStudies != value)
				{
					_filterDuplicateStudies = value;
					if (CurrentSearchResult != null)
						CurrentSearchResult.FilterDuplicates = _filterDuplicateStudies;
				}
			}
		}

		internal SearchResult CurrentSearchResult
		{
			get { return _currentSearchResult; }
            private set
            {
                if (ReferenceEquals(value, _currentSearchResult))
                    return;

                if (_currentSearchResult != null)
                    _currentSearchResult.ResultsTitleChanged -= OnResultsTitleChanged;

                _currentSearchResult = value;
                _currentSearchResult.ResultsTitleChanged += OnResultsTitleChanged;
            }
		}

		private event EventHandler SelectedStudyChanged
		{
			add { _selectedStudyChangedEvent += value; }
			remove { _selectedStudyChangedEvent -= value; }
		}

		private event EventHandler SelectedServerChanged
		{
			add { _selectedServerChangedEvent += value; }
			remove { _selectedServerChangedEvent -= value; }
		}

		#endregion

		#region Presentation Model

		public bool IsEnabled
		{
			get { return _isEnabled; }
			private set
			{
				if(value != _isEnabled)
				{
					_isEnabled = value;
					NotifyPropertyChanged("IsEnabled");
				}
			}
		}

		public Table<StudyItem> StudyTable
		{
			get
			{
				if (CurrentSearchResult == null)
					return _dummyStudyTable;
				else
					return CurrentSearchResult.StudyTable;
			}
		}

		public string ResultsTitle
		{
			get
			{
			    return CurrentSearchResult == null ? "" : CurrentSearchResult.ResultsTitle;
			}
		}

		public event EventHandler StudyTableChanged
		{
			add { _studyTableChanged += value; }
			remove { _studyTableChanged -= value; }
		}

		public StudyItem SelectedStudy
		{
			get { return _currentSelection == null ? null : _currentSelection.Item as StudyItem; }
		}

		public ReadOnlyCollection<StudyItem> SelectedStudies
		{
			get
			{
				var selectedStudies = new List<StudyItem>();
				if (_currentSelection != null)
				    selectedStudies.AddRange(_currentSelection.Items.Cast<StudyItem>());

				return selectedStudies.AsReadOnly();
			}
		}

		public ActionModelRoot ToolbarModel
		{
			get { return _toolbarModel; }
		}

		public ActionModelNode ContextMenuModel
		{
			get { return _contextMenuModel; }
		}

		public void SetSelection(ISelection selection)
		{
		    if (_currentSelection == selection)
                return;
		    
            _currentSelection = selection;
		    EventsHelper.Fire(_selectedStudyChangedEvent, this, EventArgs.Empty);
		}

		public void ItemDoubleClick()
		{
			if (_defaultActionHandler != null)
				_defaultActionHandler();
		}

		#endregion

		#region IStudyBrowserComponent implementation

		public virtual void Search(List<QueryParameters> queryParametersList)
		{
            if (_selectedServerGroup == null)
                return;

			// cancel any pending searches
			Async.CancelPending(this);

			var isOpenSearchQuery = CollectionUtils.TrueForAll(queryParametersList, q => CollectionUtils.TrueForAll(q.Values, string.IsNullOrEmpty));

			if (!_selectedServerGroup.IsLocalServer && isOpenSearchQuery)
			{
				if (Host.DesktopWindow.ShowMessageBox(SR.MessageConfirmContinueOpenSearch, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			// disable the study browser while the search is executing
			this.IsEnabled = false;
			_searchInProgress = true;

			EventsHelper.Fire(this.SearchStarted, this, EventArgs.Empty);

			_lastQueryParametersList = new List<QueryParameters>(queryParametersList);
			var failedServerInfo = new List<KeyValuePair<string, Exception>>();
			var aggregateStudyItemList = new StudyItemList();

			Async.Invoke(this,
						 () => aggregateStudyItemList = Query(queryParametersList, failedServerInfo),
						 () => OnSearchCompleted(aggregateStudyItemList, failedServerInfo));
		}

		public virtual void CancelSearch()
		{
			if(!_searchInProgress)
				return;

			Async.CancelPending(this);
			_searchInProgress = false;

			// re-enable the study browser
			this.IsEnabled = true;

			EventsHelper.Fire(this.SearchEnded, this, EventArgs.Empty);
		}

		public event EventHandler SearchStarted;
		public event EventHandler SearchEnded;

		#endregion

		#region IApplicationComponent overrides

		public override void Start()
		{
			base.Start();

			var tools = new ArrayList(new StudyBrowserToolExtensionPoint().CreateExtensions());
			tools.Add(new FilterDuplicateStudiesTool(this));
			_toolSet = new ToolSet(tools, new StudyBrowserToolContext(this));

			_toolbarModel = ActionModelRoot.CreateModel(GetType().FullName, "dicomstudybrowser-toolbar", _toolSet.Actions);
			_contextMenuModel = ActionModelRoot.CreateModel(GetType().FullName, "dicomstudybrowser-contextmenu", _toolSet.Actions);

			_searchResultColumnOptions = SearchResult.ColumnOptions;

			DicomExplorerConfigurationSettings.Default.PropertyChanged += OnConfigurationSettingsChanged;
		}

	    public override void Stop()
		{
			Async.CancelPending(this);

	        foreach (var searchResult in _searchResults.Values)
	            searchResult.Dispose();

            _toolSet.Dispose();
			_toolSet = null;

			DicomExplorerConfigurationSettings.Default.PropertyChanged -= OnConfigurationSettingsChanged;

			base.Stop();
		}

		#endregion

		#region Private Helpers

        private void OnResultsTitleChanged(object sender, EventArgs e)
        {
            UpdateResultsTitle();
        }

	    private void OnSelectedServerChanged()
		{
			CurrentSearchResult.ServerGroupName = _selectedServerGroup.Name;
			CurrentSearchResult.IsLocalServer = _selectedServerGroup.IsLocalServer;
			CurrentSearchResult.NumberOfChildServers = _selectedServerGroup.Servers.Count;
            CurrentSearchResult.FilterDuplicates = _filterDuplicateStudies;

			CurrentSearchResult.UpdateColumnVisibility();

			EventsHelper.Fire(_selectedServerChangedEvent, this, EventArgs.Empty);
			EventsHelper.Fire(_studyTableChanged, this, EventArgs.Empty);

            UpdateResultsTitle();
		}

		private void UpdateResultsTitle()
		{
			NotifyPropertyChanged("ResultsTitle");
		}

		private StudyItemList Query(List<QueryParameters> queryParamsList, ICollection<KeyValuePair<string, Exception>> failedServerInfo)
		{
			StudyItemList aggregateStudyItemList = new StudyItemList();

			foreach (IServerTreeNode serverNode in _selectedServerGroup.Servers)
			{
				var serverStudyItemList = new StudyItemList();
				var serverHasError = false;

				try
				{
					foreach (var q in queryParamsList)
					{
						// Make sure the query parameters sent contains all user specified parameters, plus any keys defined in 
                        // OpenSearchQueryParams but not in user specified parameters.

					    var openParams = new QueryParameters();
                        var queryParams = MergeQueryParams(q, openParams);

						if (serverNode.IsLocalServer)
						{
							var studyItemList = ImageViewerComponent.FindStudy(queryParams, null, "DICOM_LOCAL");
							serverStudyItemList.AddRange(studyItemList);
						}
						else if (serverNode.IsServer)
						{
                            var server = (IServerTreeDicomServer)serverNode;
                            IApplicationEntity ae = server.ToDataContract();

							var studyItemList = ImageViewerComponent.FindStudy(queryParams, ae, "DICOM_REMOTE");
							serverStudyItemList.AddRange(studyItemList);
						}
						else
						{
							throw new Exception("The specified server object is not queryable.");
						}
					}
				}
				catch (Exception e)
				{
                    Platform.Log(LogLevel.Error, e, "Failed to query server '{0}'.", serverNode.Name);
					
                    // keep track of the failed server names and exceptions
					failedServerInfo.Add(new KeyValuePair<string, Exception>(serverNode.Name, e));
					serverHasError = true;
				}

				if (!serverHasError)
					aggregateStudyItemList.AddRange(serverStudyItemList);
			}

			return aggregateStudyItemList;
		}

		private static QueryParameters MergeQueryParams(QueryParameters primary, QueryParameters secondary)
		{
			// Merge the primary with secondary query parameters.  If the key exist in both, keep the value in the primary
			// Otherwise, add the value in the secondary to the merged query parameters.
			var merged = new QueryParameters(primary);
			foreach (var k in secondary.Keys)
			{
				if (!merged.ContainsKey(k))
					merged.Add(k, secondary[k]);
			}

			return merged;
		}

		private void OnSearchCompleted(StudyItemList aggregateStudyItemList, List<KeyValuePair<string, Exception>> failedServerInfo)
		{
			CurrentSearchResult.Refresh(aggregateStudyItemList, _filterDuplicateStudies);

			if (failedServerInfo.Count > 0)
			{
				var aggregateExceptionMessage = new StringBuilder();
				var count = 0;
				foreach (var pair in failedServerInfo)
				{
					if (count++ > 0)
						aggregateExceptionMessage.Append("\n\n");

					aggregateExceptionMessage.AppendFormat(SR.FormatUnableToQueryServer, pair.Key, pair.Value.Message);
				}

				// this isn't ideal, but since we can operate on multiple entities, we need to aggregate all the
				// exception messages. We should at least attempt to get at the first inner exception, and that's
				// what we do here, to aid in debugging

				//NOTE: must use Application.ActiveDesktopWindow instead of Host.DesktopWindow b/c this
				//method is called on startup before the component is started.
				Application.ActiveDesktopWindow.ShowMessageBox(aggregateExceptionMessage.ToString(), MessageBoxActions.Ok);
			}

			_searchInProgress = false;

			// re-enable the study browser
			this.IsEnabled = true;

			EventsHelper.Fire(this.SearchEnded, this, EventArgs.Empty);
		}

        private void OnConfigurationSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
			_searchResultColumnOptions = SearchResult.ColumnOptions;
			_searchResultColumnOptions.ApplyColumnSettings(CurrentSearchResult);

			if (CurrentSearchResult != null)
				CurrentSearchResult.UpdateColumnVisibility();
		}

		#endregion
	}
}
