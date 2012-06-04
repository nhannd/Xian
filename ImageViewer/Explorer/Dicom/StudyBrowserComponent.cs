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
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Configuration.ServerTree;

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
        StudyTableItem SelectedStudy { get; }

        ReadOnlyCollection<StudyTableItem> SelectedStudies { get; }

		DicomServiceNodeList SelectedServers { get; }

		event EventHandler SelectedStudyChanged;

		event EventHandler SelectedServerChanged;

		ClickHandlerDelegate DefaultActionHandler { get; set; }

	    void RefreshStudyTable();

		IDesktopWindow DesktopWindow { get; }
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

            public StudyTableItem SelectedStudy
			{
				get
				{
					return _component.SelectedStudy;
				}
			}

            public ReadOnlyCollection<StudyTableItem> SelectedStudies
			{
				get
				{
					return _component.SelectedStudies;
				}
			}

			public DicomServiceNodeList SelectedServers
			{
				get { return _component._selectedServers; }
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
					_component.Search(_component._lastQueryCriteria);
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

		private StudyRootStudyIdentifier _lastQueryCriteria;
		private readonly Dictionary<string, SearchResult> _searchResults;
	    private SearchResult _currentSearchResult;

		private readonly Table<StudyTableItem> _dummyStudyTable;
		private event EventHandler _studyTableChanged;
		private bool _filterDuplicateStudies = true;

		private ISelection _currentSelection;
		private event EventHandler _selectedStudyChangedEvent;

		private DicomServiceNodeList _selectedServers = new DicomServiceNodeList();
		private event EventHandler _selectedServerChangedEvent;

		private ToolSet _toolSet;
		private ClickHandlerDelegate _defaultActionHandler;

		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;

		private SearchResultColumnOptionCollection _searchResultColumnOptions;

		private bool _isEnabled = true;

	    #endregion

		public StudyBrowserComponent()
		{
			_dummyStudyTable = new Table<StudyTableItem>();
			_searchResults = new Dictionary<string, SearchResult>();
            _lastQueryCriteria = new StudyRootStudyIdentifier();
		}

		#region Properties/Events

		public DicomServiceNodeList SelectedServers
		{
			get { return _selectedServers; }
			set
			{
                if (ReferenceEquals(value, _selectedServers))
                    return;

                _selectedServers = value;

			    SearchResult searchResult;
				if (!_searchResults.ContainsKey(_selectedServers.Id))
				{
					searchResult = new SearchResult();
					searchResult.Initialize();
					_searchResults.Add(_selectedServers.Id, searchResult);
				}
                else
				{
				    searchResult = _searchResults[_selectedServers.Id];
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

		public Table<StudyTableItem> StudyTable
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

        public StudyTableItem SelectedStudy
		{
            get { return _currentSelection == null ? null : _currentSelection.Item as StudyTableItem; }
		}

        public ReadOnlyCollection<StudyTableItem> SelectedStudies
		{
			get
			{
                var selectedStudies = new List<StudyTableItem>();
				if (_currentSelection != null)
                    selectedStudies.AddRange(_currentSelection.Items.Cast<StudyTableItem>());

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

		public virtual void Search(StudyRootStudyIdentifier criteria)
		{
            if (_selectedServers == null)
                return;

			// cancel any pending searches
			Async.CancelPending(this);

			if (!_selectedServers.IsLocalServer)
			{
                if (criteria.IsOpenQuery() && Host.DesktopWindow.ShowMessageBox(SR.MessageConfirmContinueOpenSearch, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			// disable the study browser while the search is executing
			this.IsEnabled = false;
		    CurrentSearchResult.SearchStarted();

			EventsHelper.Fire(this.SearchStarted, this, EventArgs.Empty);

		    _lastQueryCriteria = criteria;
			var failedServerInfo = new List<KeyValuePair<string, Exception>>();
			var aggregateStudyItemList = new List<StudyTableItem>();

			Async.Invoke(this,
                         () => aggregateStudyItemList = Query(criteria, failedServerInfo),
						 () => OnSearchCompleted(aggregateStudyItemList, failedServerInfo));
		}

	    public virtual void CancelSearch()
		{
			if(!CurrentSearchResult.SearchInProgress)
				return;

			Async.CancelPending(this);
			CurrentSearchResult.SearchCanceled();

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
			CurrentSearchResult.ServerGroupName = _selectedServers.Name;
			CurrentSearchResult.IsLocalServer = _selectedServers.IsLocalServer;
			CurrentSearchResult.NumberOfChildServers = _selectedServers.Count;
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

		private List<StudyTableItem> Query(StudyRootStudyIdentifier criteria, ICollection<KeyValuePair<string, Exception>> failedServerInfo)
		{
			var aggregateItems = new List<StudyTableItem>();

			foreach (var server in _selectedServers)
			{
				var serverItems = new List<StudyTableItem>();
				var serverHasError = false;

				try
				{
				    var storeQuery = server.IsSupported<IStudyStoreQuery>()
				                         ? server.GetService<IStudyStoreQuery>()
				                         : new StudyRootQueryStoreAdapter(server.GetService<IStudyRootQuery>());
                    
                        using (var bridge = new StudyStoreBridge(storeQuery))
                            serverItems = bridge.GetStudyEntries(criteria).Select(e => new StudyTableItem(e)).ToList();
				}
				catch (Exception e)
				{
                    Platform.Log(LogLevel.Error, e, "Failed to query server '{0}'.", server.Name);
					
                    // keep track of the failed server names and exceptions
					failedServerInfo.Add(new KeyValuePair<string, Exception>(server.Name, e));
					serverHasError = true;
				}

				if (!serverHasError)
					aggregateItems.AddRange(serverItems);
			}

			return aggregateItems;
		}

		private void OnSearchCompleted(List<StudyTableItem> aggregateItems, List<KeyValuePair<string, Exception>> failedServerInfo)
		{
			CurrentSearchResult.SearchEnded(aggregateItems, _filterDuplicateStudies);

            // re-enable the study browser
            this.IsEnabled = true;
            EventsHelper.Fire(this.SearchEnded, this, EventArgs.Empty);
            
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
