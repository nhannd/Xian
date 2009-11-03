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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.Services.ServerTree;
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
	public class StudyBrowserComponent : ApplicationComponent
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
					BlockingOperation.Run(_component.Search);
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

		private SearchPanelComponent _searchPanelComponent;

		private readonly Dictionary<string, SearchResult> _searchResults;
		private readonly Table<StudyItem> _dummyStudyTable;
		private event EventHandler _studyTableChanged;
		private bool _filterDuplicateStudies = true;

		private ISelection _currentSelection;
		private event EventHandler _selectedStudyChangedEvent;

		private AEServerGroup _selectedServerGroup;
		private event EventHandler _selectedServerChangedEvent;

		private ToolSet _toolSet;
		private OpenStudyTool _openStudyTool;
		private ClickHandlerDelegate _defaultActionHandler;

		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;

		private bool _localDataStoreCleared;
		private readonly Dictionary<string, string> _setStudiesArrived;
		private readonly Dictionary<string, string> _setStudiesDeleted;

		private ILocalDataStoreEventBroker _localDataStoreEventBroker;
		private DelayedEventPublisher _processStudiesEventPublisher;

		#endregion

		public StudyBrowserComponent()
		{
			_dummyStudyTable = new Table<StudyItem>();
			_searchResults = new Dictionary<string, SearchResult>();

			_localDataStoreCleared = false;
			_setStudiesArrived = new Dictionary<string, string>();
			_setStudiesDeleted = new Dictionary<string, string>();
		}

		#region Properties/Events

		internal SearchPanelComponent SearchPanelComponent
		{
			get { return _searchPanelComponent; }
			set { _searchPanelComponent = value; }
		}

		internal AEServerGroup SelectedServerGroup
		{
			get { return _selectedServerGroup; }
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
					{
						CurrentSearchResult.Refresh(_filterDuplicateStudies);
						UpdateResultsTitle();
					}
				}
			}
		}

		internal SearchResult CurrentSearchResult
		{
			get
			{
				if (_selectedServerGroup == null || !_searchResults.ContainsKey(_selectedServerGroup.GroupID))
					return null;

				return _searchResults[_selectedServerGroup.GroupID];
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

		#region Presentation Model

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
				if (CurrentSearchResult == null)
					return "";
				else
					return CurrentSearchResult.ResultsTitle;
			}
		}

		public event EventHandler StudyTableChanged
		{
			add { _studyTableChanged += value; }
			remove { _studyTableChanged -= value; }
		}

		public StudyItem SelectedStudy
		{
			get
			{
				if (_currentSelection == null)
					return null;
				else
					return _currentSelection.Item as StudyItem;
			}
		}

		public ReadOnlyCollection<StudyItem> SelectedStudies
		{
			get
			{
				List<StudyItem> selectedStudies = new List<StudyItem>();

				if (_currentSelection != null)
				{
					foreach (StudyItem item in _currentSelection.Items)
						selectedStudies.Add(item);
				}

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

		public void SelectServerGroup(AEServerGroup selectedServerGroup)
		{
			_selectedServerGroup = selectedServerGroup;

			if (!_searchResults.ContainsKey(_selectedServerGroup.GroupID))
			{
				SearchResult searchResult = new SearchResult();
				_searchResults.Add(_selectedServerGroup.GroupID, searchResult);
			}

			ProcessReceivedAndRemovedStudies();
			OnSelectedServerChanged();
		}

		public void SetSelection(ISelection selection)
		{
			if (_currentSelection != selection)
			{
				_currentSelection = selection;
				EventsHelper.Fire(_selectedStudyChangedEvent, this, EventArgs.Empty);
			}
		}

		public void Search()
		{
			if (_selectedServerGroup != null && _selectedServerGroup.IsLocalDatastore)
				_setStudiesArrived.Clear();

			QueryParameters queryParams = PrepareQueryParameters();

			bool isOpenSearchQuery = (queryParams["PatientsName"].Length == 0
									  && queryParams["ReferringPhysiciansName"].Length == 0
									  && queryParams["PatientId"].Length == 0
			                          && queryParams["AccessionNumber"].Length == 0
			                          && queryParams["StudyDescription"].Length == 0
			                          && queryParams["ModalitiesInStudy"].Length == 0
			                          && queryParams["StudyDate"].Length == 0 &&
			                          queryParams["StudyInstanceUid"].Length == 0);

			if (!_selectedServerGroup.IsLocalDatastore && isOpenSearchQuery)
			{
				if (Host.DesktopWindow.ShowMessageBox(SR.MessageConfirmContinueOpenSearch, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			List<KeyValuePair<string, Exception>> failedServerInfo = new List<KeyValuePair<string, Exception>>();
			StudyItemList aggregateStudyItemList = Query(queryParams, failedServerInfo);

			CurrentSearchResult.Refresh(aggregateStudyItemList, _filterDuplicateStudies);

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

				//NOTE: must use Application.ActiveDesktopWindow instead of Host.DesktopWindow b/c this
				//method is called on startup before the component is started.
				Application.ActiveDesktopWindow.ShowMessageBox(aggregateExceptionMessage.ToString(), MessageBoxActions.Ok);
			}

			UpdateResultsTitle();
		}

		public void ItemDoubleClick()
		{
			if (_defaultActionHandler != null)
			{
				_defaultActionHandler();
			}
			else if (_openStudyTool != null)
			{
				//TODO (cr Oct 2009): get rid of explicit reference.
				//fall back to the open study tool.
				_openStudyTool.OpenStudy();
			}
		}

		#endregion
		#endregion

		#region IApplicationComponent overrides

		public override void Start()
		{
			base.Start();

			_processStudiesEventPublisher = new DelayedEventPublisher(DelayProcessReceivedAndRemoved);

			ArrayList tools = new ArrayList(new StudyBrowserToolExtensionPoint().CreateExtensions());
			tools.Add(_openStudyTool = new OpenStudyTool());
			tools.Add(new FilterDuplicateStudiesTool(this));
			_toolSet = new ToolSet(tools, new StudyBrowserToolContext(this));

			_toolbarModel = ActionModelRoot.CreateModel(GetType().FullName, "dicomstudybrowser-toolbar", _toolSet.Actions);
			_contextMenuModel = ActionModelRoot.CreateModel(GetType().FullName, "dicomstudybrowser-contextmenu", _toolSet.Actions);

			_localDataStoreEventBroker = LocalDataStoreActivityMonitor.CreatEventBroker();
			_localDataStoreEventBroker.SopInstanceImported += OnSopInstanceImported;
			_localDataStoreEventBroker.InstanceDeleted += OnInstanceDeleted;
			_localDataStoreEventBroker.LocalDataStoreCleared += OnLocalDataStoreCleared;

			DicomExplorerConfigurationSettings.Default.PropertyChanged += OnConfigurationSettingsChanged;
		}


		public override void Stop()
		{
			_processStudiesEventPublisher.Dispose();

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

		private void OnSelectedServerChanged()
		{
			CurrentSearchResult.ServerGroupName = _selectedServerGroup.Name;
			CurrentSearchResult.IsLocalDataStore = _selectedServerGroup.IsLocalDatastore;
			CurrentSearchResult.NumberOfChildServers = _selectedServerGroup.Servers.Count;

			CurrentSearchResult.UpdateColumnVisibility();
			CurrentSearchResult.Refresh(_filterDuplicateStudies);

			EventsHelper.Fire(_selectedServerChangedEvent, this, EventArgs.Empty);
			EventsHelper.Fire(_studyTableChanged, this, EventArgs.Empty);
			UpdateResultsTitle();
		}

		private void UpdateResultsTitle()
		{
			NotifyPropertyChanged("ResultsTitle");
		}

		private QueryParameters PrepareQueryParameters()
		{
			Platform.CheckMemberIsSet(_searchPanelComponent, "SearchPanelComponent");

			string patientsName = ConvertNameToSearchCriteria(_searchPanelComponent.PatientsName);
			string referringPhysiciansName = ConvertNameToSearchCriteria(_searchPanelComponent.ReferringPhysiciansName);

			string patientId = "";
			if (!String.IsNullOrEmpty(_searchPanelComponent.PatientID))
				patientId = _searchPanelComponent.PatientID + "*";

			string accessionNumber = "";
			if (!String.IsNullOrEmpty(_searchPanelComponent.AccessionNumber))
				accessionNumber = _searchPanelComponent.AccessionNumber + "*";

			string studyDescription = "";
			if (!String.IsNullOrEmpty(_searchPanelComponent.StudyDescription))
				studyDescription = _searchPanelComponent.StudyDescription + "*";

			string dateRangeQuery = DateRangeHelper.GetDicomDateRangeQueryString(_searchPanelComponent.StudyDateFrom, _searchPanelComponent.StudyDateTo);

			//At the application level, ClearCanvas defines the 'ModalitiesInStudy' filter as a multi-valued
			//Key Attribute.  This goes against the Dicom standard for C-FIND SCU behaviour, so the
			//underlying IStudyFinder(s) must handle this special case, either by ignoring the filter
			//or by running multiple queries, one per modality specified (for example).

			string modalityFilter = DicomStringHelper.GetDicomStringArray(_searchPanelComponent.SearchModalities);

			QueryParameters queryParams = new QueryParameters();
			queryParams.Add("PatientsName", patientsName);
			queryParams.Add("ReferringPhysiciansName", referringPhysiciansName); 
			queryParams.Add("PatientId", patientId);
			queryParams.Add("AccessionNumber", accessionNumber);
			queryParams.Add("StudyDescription", studyDescription);
			queryParams.Add("ModalitiesInStudy", modalityFilter);
			queryParams.Add("StudyDate", dateRangeQuery);
			queryParams.Add("StudyInstanceUid", "");

			return queryParams;
		}

		private static string[] GetNameComponents(string unparsedName)
		{
			unparsedName = unparsedName ?? "";
			char separator = DicomExplorerConfigurationSettings.Default.NameSeparator;
			string name = unparsedName.Trim();
			if (String.IsNullOrEmpty(name))
				return new string[0];

			return name.Split(new char[] { separator }, StringSplitOptions.None);
		}

		private static string ConvertNameToSearchCriteria(string name)
		{
			string[] nameComponents = GetNameComponents(name);
			if (nameComponents.Length == 1)
			{
				//Open name search
				return String.Format("*{0}*", nameComponents[0].Trim());
			}
			else if (nameComponents.Length > 1)
			{
				if (String.IsNullOrEmpty(nameComponents[0]))
				{
					//Open name search - should never get here
					return String.Format("*{0}*", nameComponents[1].Trim());
				}
				else if (String.IsNullOrEmpty(nameComponents[1]))
				{
					//Pure Last Name search
					return String.Format("{0}*", nameComponents[0].Trim());
				}
				else
				{
					//Last Name, First Name search
					return String.Format("{0}*{1}*", nameComponents[0].Trim(), nameComponents[1].Trim());
				}
			}

			return "";
		}

		private StudyItemList Query(QueryParameters queryParams, ICollection<KeyValuePair<string, Exception>> failedServerInfo)
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
						ApplicationEntity ae = new ApplicationEntity(server.Host, server.AETitle, server.Name, server.Port,
														server.IsStreaming, server.HeaderServicePort, server.WadoServicePort);

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

		private bool StudyExists(string studyInstanceUid)
		{
			return GetStudyIndex(studyInstanceUid) >= 0;
		}

		private int GetStudyIndex(string studyInstanceUid)
		{
			return StudyTable.Items.FindIndex(
				delegate(StudyItem test)
				{
					return test.StudyInstanceUid == studyInstanceUid;
				});
		}

		private void ProcessReceivedAndRemovedStudies()
		{
			if (_selectedServerGroup == null || !_selectedServerGroup.IsLocalDatastore)
				return;

			Table<StudyItem> studyTable = StudyTable;

			if (_localDataStoreCleared)
			{
				_localDataStoreCleared = false;
				studyTable.Items.Clear();
			}

			if (_setStudiesArrived.Count > 0)
			{
				List<string> studyUidList = new List<string>();
				foreach (string studyUid in _setStudiesArrived.Keys)
					studyUidList.Add(studyUid);

				string studyUids = DicomStringHelper.GetDicomStringArray(studyUidList);
				if (!String.IsNullOrEmpty(studyUids))
				{
					QueryParameters parameters = PrepareQueryParameters();
					parameters["StudyInstanceUid"] = studyUids;

					try
					{
						StudyItemList list = ImageViewerComponent.FindStudy(parameters, null, "DICOM_LOCAL");
						foreach (StudyItem item in list)
						{
							//don't need to check this again, it's just paranoia
							if (!StudyExists(item.StudyInstanceUid))
							{
								studyTable.Items.Add(item);
							}
							else
							{
								int index = GetStudyIndex(item.StudyInstanceUid);
								//just update this since the rest won't change.
								UpdateItem(studyTable.Items[index], item);
								studyTable.Items.NotifyItemUpdated(index);
							}
						}
					}
					catch(StudyFinderNotFoundException e)
					{
						//should never get here, really.
						Platform.Log(LogLevel.Error, e);
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
					}
				}
			}

			foreach(string deleteStudyUid in _setStudiesDeleted.Keys)
			{
				int foundIndex = studyTable.Items.FindIndex(
				delegate(StudyItem test)
				{
					return test.StudyInstanceUid == deleteStudyUid;
				});

				if (foundIndex >= 0)
				{
					studyTable.Items.RemoveAt(foundIndex);
				}
			}

			_setStudiesArrived.Clear();
			_setStudiesDeleted.Clear();
		}

		private void DelayProcessReceivedAndRemoved(object sender, EventArgs e)
		{
			ProcessReceivedAndRemovedStudies();
			UpdateResultsTitle();
		}

		private void OnSopInstanceImported(object sender, ItemEventArgs<ImportedSopInstanceInformation> e)
		{
			if (_setStudiesArrived.ContainsKey(e.Item.StudyInstanceUid))
				return;

			_setStudiesArrived[e.Item.StudyInstanceUid] = e.Item.StudyInstanceUid;
			_setStudiesDeleted.Remove(e.Item.StudyInstanceUid); //can't be deleted if it's arrived.

			_processStudiesEventPublisher.Publish(this, EventArgs.Empty);
		}

		private void OnInstanceDeleted(object sender, ItemEventArgs<DeletedInstanceInformation> e)
		{
			if (e.Item.InstanceLevel != InstanceLevel.Study)
				return;

			if (e.Item.Failed)
				return;

			_setStudiesDeleted[e.Item.InstanceUid] = e.Item.InstanceUid;
			_setStudiesArrived.Remove(e.Item.InstanceUid); //can't arrive if it's deleted.

			_processStudiesEventPublisher.Publish(this, EventArgs.Empty);
		}

		private void OnLocalDataStoreCleared(object sender, EventArgs e)
		{
			_setStudiesArrived.Clear();
			_setStudiesDeleted.Clear();
			_localDataStoreCleared = true;

			ProcessReceivedAndRemovedStudies();
			UpdateResultsTitle();
		}

		private void OnConfigurationSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
			if (CurrentSearchResult != null)
				CurrentSearchResult.UpdateColumnVisibility();
		}

		private static void UpdateItem(StudyItem existingItem, StudyItem sourceItem)
		{
			//TODO: later, make each item have a 'changed' event for the properties instead of doing this
			existingItem.AccessionNumber = sourceItem.AccessionNumber;
			existingItem.ReferringPhysiciansName = sourceItem.ReferringPhysiciansName;
			existingItem.ModalitiesInStudy = sourceItem.ModalitiesInStudy;
			existingItem.NumberOfStudyRelatedInstances = sourceItem.NumberOfStudyRelatedInstances;
			existingItem.PatientId = sourceItem.PatientId;
			existingItem.PatientsName = sourceItem.PatientsName;
			existingItem.PatientsBirthDate = sourceItem.PatientsBirthDate;
			existingItem.SpecificCharacterSet = sourceItem.SpecificCharacterSet;
			existingItem.StudyDate = sourceItem.StudyDate;
			existingItem.StudyDescription = sourceItem.StudyDescription;
		}
	}
}
