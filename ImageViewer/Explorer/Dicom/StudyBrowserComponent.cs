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
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.Collections.ObjectModel;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint()]
	public sealed class StudyBrowserToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint()]
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

		void RefreshStudyList();
	}
	
	[AssociateView(typeof(StudyBrowserComponentViewExtensionPoint))]
	public class StudyBrowserComponent : ApplicationComponent
	{
		public class StudyBrowserToolContext : ToolContext, IStudyBrowserToolContext
		{
			StudyBrowserComponent _component;

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

			public void RefreshStudyList()
			{
				_component.Search();
			}

			#endregion
		}

		public class SearchResult
		{
			private Table<StudyItem> _studyList;
			private string _resultsTitle = "";

			public SearchResult()
			{

			}

			public Table<StudyItem> StudyList
			{
				get 
				{
					if (_studyList == null)
						_studyList = new Table<StudyItem>();

					return _studyList; 
				}
			}

			public string ResultsTitle
			{
				get { return _resultsTitle; }
				set { _resultsTitle = value; }
			}
		}
	
		#region Fields

		private SearchPanelComponent _searchPanelComponent;

		private Dictionary<string, SearchResult> _searchResults;
		private Table<StudyItem> _currentStudyList;
		
		private string _resultsTitle;

		private ISelection _currentSelection;
		private event EventHandler _selectedStudyChangedEvent;
		private ClickHandlerDelegate _defaultActionHandler;
		private ToolSet _toolSet;

		private AEServerGroup _selectedServerGroup;
		private event EventHandler _selectedServerChangedEvent;

		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;

		private bool _localDataStoreCleared;
		private Dictionary<string, string> _setStudiesArrived;
		private Dictionary<string, string> _setStudiesDeleted;

		private ILocalDataStoreEventBroker _localDataStoreEventBroker;

		#endregion

		public StudyBrowserComponent()
		{
			_searchResults = new Dictionary<string, SearchResult>();
			_localDataStoreCleared = false;
			_setStudiesArrived = new Dictionary<string, string>();
			_setStudiesDeleted = new Dictionary<string, string>();
		}

		internal SearchPanelComponent SearchPanelComponent
		{
			get { return _searchPanelComponent; }
			set { _searchPanelComponent = value; }
		}

		public Table<StudyItem> StudyList
		{
			get { return _currentStudyList; }
			set { _currentStudyList = value; }
		}

		public StudyItem SelectedStudy
		{
			get
			{
				if (_currentSelection == null)
					return null;

				return _currentSelection.Item as StudyItem;
			}
		}

		public ReadOnlyCollection<StudyItem> SelectedStudies
		{
			get
			{
				if (_currentSelection == null)
					return null;

				List<StudyItem> selectedStudies = new List<StudyItem>();

				foreach (StudyItem item in _currentSelection.Items)
					selectedStudies.Add(item);

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

		public string ResultsTitle
		{
			get { return _resultsTitle; }
			set
			{
				_resultsTitle = value;
				NotifyPropertyChanged("ResultsTitle");
			}
		}

		private event EventHandler SelectedStudyChanged
		{
			add { _selectedStudyChangedEvent += value; }
			remove { _selectedStudyChangedEvent -= value; }
		}

		public event EventHandler SelectedServerChanged
		{
			add { _selectedServerChangedEvent += value; }
			remove { _selectedServerChangedEvent -= value; }
		}

		#region IApplicationComponent overrides

		public override void Start()
		{
			base.Start();

			_toolSet = new ToolSet(new StudyBrowserToolExtensionPoint(), new StudyBrowserToolContext(this));
			_toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomstudybrowser-toolbar", _toolSet.Actions);
			_contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomstudybrowser-contextmenu", _toolSet.Actions);

			_localDataStoreEventBroker = LocalDataStoreActivityMonitor.CreatEventBroker(true);
			_localDataStoreEventBroker.SopInstanceImported += OnSopInstanceImported;
			_localDataStoreEventBroker.InstanceDeleted += OnInstanceDeleted;
			_localDataStoreEventBroker.LocalDataStoreCleared += OnLocalDataStoreCleared;

			DicomExplorerConfigurationSettings.Default.PropertyChanged += OnConfigurationSettingsChanged;
		}

		public override void Stop()
		{
			_toolSet.Dispose();
			_toolSet = null;

			_localDataStoreEventBroker.SopInstanceImported -= OnSopInstanceImported;
			_localDataStoreEventBroker.InstanceDeleted -= OnInstanceDeleted;
			_localDataStoreEventBroker.LocalDataStoreCleared -= OnLocalDataStoreCleared;
			_localDataStoreEventBroker.Dispose();

			DicomExplorerConfigurationSettings.Default.PropertyChanged -= OnConfigurationSettingsChanged;

			base.Stop();
		}

		#endregion

		public void SelectServerGroup(AEServerGroup selectedServerGroup)
		{
			_selectedServerGroup = selectedServerGroup;

			if (!_searchResults.ContainsKey(_selectedServerGroup.GroupID))
			{
				SearchResult searchResult = new SearchResult();
				searchResult.ResultsTitle = String.Format("{0}", _selectedServerGroup.Name);
				AddColumns(searchResult.StudyList);

				_searchResults.Add(_selectedServerGroup.GroupID, searchResult);
			}

			ProcessReceivedAndRemovedStudies();

			//Update both of these in the view.
			this.ResultsTitle = _searchResults[_selectedServerGroup.GroupID].ResultsTitle;
			this.StudyList = _searchResults[_selectedServerGroup.GroupID].StudyList;

			EventsHelper.Fire(_selectedServerChangedEvent, this, EventArgs.Empty);
		}

		public void Search()
		{
			if (_selectedServerGroup != null && _selectedServerGroup.IsLocalDatastore)
				_setStudiesArrived.Clear();

			QueryParameters queryParams = PrepareQueryParameters();

			bool isOpenSearchQuery = (queryParams["PatientsName"].Length == 0
							&& queryParams["PatientId"].Length == 0
							&& queryParams["AccessionNumber"].Length == 0
							&& queryParams["StudyDescription"].Length == 0
							&& queryParams["ModalitiesInStudy"].Length == 0
							&& queryParams["StudyDate"].Length == 0 &&
							queryParams["StudyInstanceUid"].Length == 0);


			if (!_selectedServerGroup.IsLocalDatastore && isOpenSearchQuery)
			{
				if (this.Host.DesktopWindow.ShowMessageBox(SR.MessageConfirmContinueOpenSearch, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			List<KeyValuePair<string, Exception>> failedServerInfo = new List<KeyValuePair<string, Exception>>();
			StudyItemList aggregateStudyItemList = Query(queryParams, failedServerInfo);

			this.ResultsTitle = String.Format(SR.FormatStudiesFound, aggregateStudyItemList.Count, _selectedServerGroup.Name);

			//Update the results title in the component and add the new results.
			_searchResults[_selectedServerGroup.GroupID].ResultsTitle = this.ResultsTitle;
			_searchResults[_selectedServerGroup.GroupID].StudyList.Items.Clear();
			
			foreach (StudyItem item in aggregateStudyItemList)
				_searchResults[_selectedServerGroup.GroupID].StudyList.Items.Add(item);

			_searchResults[_selectedServerGroup.GroupID].StudyList.Sort();

            // Re-throw the last exception with a list of failed server name, if any
			if (failedServerInfo.Count > 0)
            {
				StringBuilder aggregateExceptionMessage = new StringBuilder();
				int count = 0;
                foreach(KeyValuePair<string, Exception> pair in failedServerInfo)
                {
					if (count++ > 0)
						aggregateExceptionMessage.Append("\n\n");

					aggregateExceptionMessage.AppendFormat(SR.FormatUnableToQueryServer, pair.Key, pair.Value.Message);
                }

                // this isn't ideal, but since we can operate on multiple entities, we need to aggregate all the
                // exception messages. We should at least attempt to get at the first inner exception, and that's
                // what we do here, to aid in debugging
                this.Host.DesktopWindow.ShowMessageBox(aggregateExceptionMessage.ToString(), MessageBoxActions.Ok);
            }
		}

		public void ItemDoubleClick()
		{
			if (_defaultActionHandler != null)
			{
				_defaultActionHandler();
			}
		}

		public void SetSelection(ISelection selection)
		{
			if (_currentSelection != selection)
			{
				_currentSelection = selection;
				EventsHelper.Fire(_selectedStudyChangedEvent, this, EventArgs.Empty);
			}
		}

		private QueryParameters PrepareQueryParameters()
		{
			Platform.CheckMemberIsSet(_searchPanelComponent, "SearchPanelComponent");

			string firstName = _searchPanelComponent.AllowFirstName ? _searchPanelComponent.FirstName : "";

			// create patient's name query key
			// LastName   FirstName   Result
			//    X           X        <Blank>
			//    V           X        LastName*
			//    V           V        LastName*FirstName*
			//    X           V        *FirstName*
			string patientsName = "";
			if (_searchPanelComponent.LastName.Length > 0 && firstName.Length == 0)
				patientsName = _searchPanelComponent.LastName + "*";
			if (_searchPanelComponent.LastName.Length > 0 && firstName.Length > 0)
				patientsName = _searchPanelComponent.LastName + "*" + firstName + "*";
			if (_searchPanelComponent.LastName.Length == 0 && firstName.Length > 0)
				patientsName = "*" + firstName + "*";

			string patientId = "";
			if (_searchPanelComponent.PatientID.Length > 0)
				patientId = _searchPanelComponent.PatientID + "*";

			string accessionNumber = "";
			if (_searchPanelComponent.AccessionNumber.Length > 0)
				accessionNumber = _searchPanelComponent.AccessionNumber + "*";

			string studyDescription = "";
			if (_searchPanelComponent.StudyDescription.Length > 0)
				studyDescription = _searchPanelComponent.StudyDescription + "*";

			string dateRangeQuery = DateRangeHelper.GetDicomDateRangeQueryString(_searchPanelComponent.StudyDateFrom, _searchPanelComponent.StudyDateTo);

			//At the application level, ClearCanvas defines the 'ModalitiesInStudy' filter as a multi-valued
			//Key Attribute.  This goes against the Dicom standard for C-FIND SCU behaviour, so the
			//underlying IStudyFinder(s) must handle this special case, either by ignoring the filter
			//or by running multiple queries, one per modality specified (for example).

			string modalityFilter = DicomStringHelper.GetDicomStringArray<string>(_searchPanelComponent.SearchModalities);

			QueryParameters queryParams = new QueryParameters();
			queryParams.Add("PatientsName", patientsName);
			queryParams.Add("PatientId", patientId);
			queryParams.Add("AccessionNumber", accessionNumber);
			queryParams.Add("StudyDescription", studyDescription);
			queryParams.Add("ModalitiesInStudy", modalityFilter);
			queryParams.Add("StudyDate", dateRangeQuery);
			queryParams.Add("StudyInstanceUid", "");

			return queryParams;
		}

		private StudyItemList Query(QueryParameters queryParams, List<KeyValuePair<string, Exception>> failedServerInfo)
		{
			StudyItemList aggregateStudyItemList = new StudyItemList();

			foreach (IServerTreeNode serverNode in _selectedServerGroup.Servers)
			{
				try
				{
					StudyItemList serverStudyItemList;

					if (serverNode.IsLocalDataStore)
					{
						serverStudyItemList = ImageViewerComponent.FindStudy(queryParams, null, "DICOM_LOCAL");
					}
					else if (serverNode.IsServer)
					{
						Server server = (Server)serverNode;
						ApplicationEntity ae = new ApplicationEntity(
							server.Host, 
							server.AETitle, 
							server.Port,
							server.HeaderServicePort,
							server.WadoServicePort);

						serverStudyItemList = ImageViewerComponent.FindStudy(queryParams, ae, "DICOM_REMOTE");
					}
					else
					{
						throw new Exception("The specified server object is not queryable.");
					}

					aggregateStudyItemList.AddRange(serverStudyItemList);
				}
				catch (Exception e)
				{
					// keep track of the failed server names and exceptions
					failedServerInfo.Add(new KeyValuePair<string, Exception>(serverNode.Name, e));
				}
			}

			return aggregateStudyItemList;
		}

		private void AddColumns(Table<StudyItem> studyList)
		{
			TableColumn<StudyItem, string> column;

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingPatientId,
					delegate(StudyItem item) { return item.PatientId; },
					1.5f);

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingLastName,
					delegate(StudyItem item) { return item.PatientsName.LastName; },
                    1.5f);

			studyList.Columns.Add(column);

			// Default: Sort by lastname
			studyList.Sort(new TableSortParams(column, true));

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingFirstName,
					delegate(StudyItem item) { return item.PatientsName.FirstName; },
                    1.5f);

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingIdeographicName,
					delegate(StudyItem item) { return item.PatientsName.Ideographic; },
					1.5f);

			column.Visible = DicomExplorerConfigurationSettings.Default.ShowIdeographicName;

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingPhoneticName,
					delegate(StudyItem item) { return item.PatientsName.Phonetic; },
					1.5f);

			column.Visible = DicomExplorerConfigurationSettings.Default.ShowPhoneticName;

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingDateOfBirth,
                    delegate(StudyItem item) { return GetDateStringFromDicomDA(item.PatientsBirthDate); },
                    null,
                    1.0f,
                    delegate(StudyItem one, StudyItem two) { return one.PatientsBirthDate.CompareTo(two.PatientsBirthDate); });

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingAccessionNumber,
					delegate(StudyItem item) { return item.AccessionNumber; });

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingStudyDate,
					delegate(StudyItem item){ return GetDateStringFromDicomDA(item.StudyDate); },
                    null,
                    1.0f,
                    delegate(StudyItem one, StudyItem two) {  return one.StudyDate.CompareTo(two.StudyDate); });

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingStudyDescription,
					delegate(StudyItem item) { return item.StudyDescription; },
                    2.5f);

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingModality,
                    delegate(StudyItem item) { return item.ModalitiesInStudy; },
                    0.5f);

			studyList.Columns.Add(column);
		}

		private string GetDateStringFromDicomDA(string dicomDate)
		{
			DateTime date;
			if (!DateParser.Parse(dicomDate, out date))
				return dicomDate;

			return date.ToString(Format.DateFormat);
		}

		private bool StudyExists(string studyInstanceUid)
		{
			return GetStudyIndex(studyInstanceUid) >= 0;
		}

		private int GetStudyIndex(string studyInstanceUid)
		{
			return _searchResults[_selectedServerGroup.GroupID].StudyList.Items.FindIndex(
				delegate(StudyItem test)
				{
					return test.StudyInstanceUID == studyInstanceUid;
				});
		}

		private void ProcessReceivedAndRemovedStudies()
		{
			if (_selectedServerGroup == null || !_selectedServerGroup.IsLocalDatastore)
				return;

			Table<StudyItem> studyTable = _searchResults[_selectedServerGroup.GroupID].StudyList;
			bool refreshTitle = false;

			if (_localDataStoreCleared)
			{
				refreshTitle = true;
				_localDataStoreCleared = false;
				studyTable.Items.Clear();
			}

			List<string> studyUidList = new List<string>();
			foreach (string studyUid in _setStudiesArrived.Keys)
				studyUidList.Add(studyUid);

			string studyUids = DicomStringHelper.GetDicomStringArray<string>(studyUidList);
			if (String.IsNullOrEmpty(studyUids))
				return;

			QueryParameters parameters = PrepareQueryParameters();
			parameters["StudyInstanceUid"] = studyUids;

			try
			{
				StudyItemList list = ImageViewerComponent.FindStudy(parameters, null, "DICOM_LOCAL");
				foreach (StudyItem item in list)
				{
					//don't need to check this again, it's just paranoia
					if (!StudyExists(item.StudyInstanceUID))
					{
						studyTable.Items.Add(item);
						refreshTitle = true;
					}
					else
					{
						int index = GetStudyIndex(item.StudyInstanceUID);
						//just update this since the rest won't change.
						studyTable.Items[index].ModalitiesInStudy = item.ModalitiesInStudy;
						studyTable.Items.NotifyItemUpdated(index);
					}
				}
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			foreach(string deleteStudyUid in _setStudiesDeleted.Keys)
			{
				int foundIndex = studyTable.Items.FindIndex(
				delegate(StudyItem test)
				{
					return test.StudyInstanceUID == deleteStudyUid;
				});

				if (foundIndex >= 0)
				{
					studyTable.Items.RemoveAt(foundIndex);
					refreshTitle = true;
				}
			}

			_setStudiesArrived.Clear();
			_setStudiesDeleted.Clear();

			if (refreshTitle)
			{
				//update the search results title.
				_searchResults[_selectedServerGroup.GroupID].ResultsTitle =
					String.Format(SR.FormatStudiesFound, _searchResults[_selectedServerGroup.GroupID].StudyList.Items.Count,
					              _selectedServerGroup.Name);
			}
		}

		private void OnSopInstanceImported(object sender, ItemEventArgs<ImportedSopInstanceInformation> e)
		{
			if (_setStudiesArrived.ContainsKey(e.Item.StudyInstanceUid))
				return;

			_setStudiesArrived[e.Item.StudyInstanceUid] = e.Item.StudyInstanceUid;
			_setStudiesDeleted.Remove(e.Item.StudyInstanceUid); //can't be deleted if it's arrived.
			ProcessReceivedAndRemovedStudies();

			//update the title in the view.
			this.ResultsTitle = _searchResults[_selectedServerGroup.GroupID].ResultsTitle;
		}

		private void OnInstanceDeleted(object sender, ItemEventArgs<DeletedInstanceInformation> e)
		{
			if (e.Item.InstanceLevel != InstanceLevel.Study)
				return;

			if (e.Item.Failed)
				return;

			_setStudiesDeleted[e.Item.InstanceUid] = e.Item.InstanceUid;
			_setStudiesArrived.Remove(e.Item.InstanceUid); //can't arrive if it's deleted.
			ProcessReceivedAndRemovedStudies();

			//update the title in the view.
			this.ResultsTitle = _searchResults[_selectedServerGroup.GroupID].ResultsTitle;
		}

		void OnLocalDataStoreCleared(object sender, EventArgs e)
		{
			_setStudiesArrived.Clear();
			_setStudiesDeleted.Clear();
			_localDataStoreCleared = true;

			ProcessReceivedAndRemovedStudies();

			//update the title in the view.
			this.ResultsTitle = _searchResults[_selectedServerGroup.GroupID].ResultsTitle;
		}

		private void OnConfigurationSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ShowIdeographicName" ||
				e.PropertyName == "ShowPhoneticName")
			{
				// Iterate through all the tables from all servers and turn off
				// the appropriate columns.
				foreach (SearchResult result in _searchResults.Values)
				{
					foreach (TableColumnBase<StudyItem> column in result.StudyList.Columns)
					{
						if (column.Name == SR.ColumnHeadingPhoneticName ||
							column.Name == SR.ColumnHeadingIdeographicName)
							column.Visible = DicomExplorerConfigurationSettings.Default.ShowIdeographicName;
					}
				}
			}
		}
	}
}
