using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Network;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint()]
	public class StudyBrowserToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint()]
	public class StudyBrowserComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public interface IStudyBrowserToolContext : IToolContext
	{
		StudyItem SelectedStudy { get; }

		IEnumerable<StudyItem> SelectedStudies { get; }

		AEServer SelectedServer { get; }

		event EventHandler SelectedStudyChanged;

		event EventHandler SelectedServerChanged;

		ClickHandlerDelegate DefaultActionHandler { get; set; }

		IDesktopWindow DesktopWindow { get; }

		void RefreshStudyList();
	}
	
	[AssociateView(typeof(StudyBrowserComponentViewExtensionPoint))]
	public class StudyBrowserComponent : ApplicationComponent, INotifyPropertyChanged
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
					if (_component._currentSelection == null)
						return null;

					return _component._currentSelection.Item as StudyItem; 
				}
			}

			public IEnumerable<StudyItem> SelectedStudies 
			{
				get 
				{
					if (_component._currentSelection == null)
						return null;

					List<StudyItem> selectedStudies = new List<StudyItem>();

					foreach (StudyItem item in _component._currentSelection.Items)
						selectedStudies.Add(item);

					return selectedStudies;
				} 
			}

			public AEServer SelectedServer
			{
				get { return _component._selectedServer; }
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
			private TableData<StudyItem> _studyList;
			private string _resultsTitle = "";

			public SearchResult()
			{

			}

			public TableData<StudyItem> StudyList
			{
				get 
				{
					if (_studyList == null)
						_studyList = new TableData<StudyItem>();

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

		private IStudyFinder _studyFinder;
		private Dictionary<string, SearchResult> _searchResults;
		private TableData<StudyItem> _currentStudyList;

		private string _resultsTitle;

		private ISelection _currentSelection;
		private event EventHandler _selectedStudyChangedEvent;
		private ClickHandlerDelegate _defaultActionHandler;
		private ToolSet _toolSet;

		private AEServer _selectedServer;
		private event EventHandler _selectedServerChangedEvent;

		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;



		#endregion

		public StudyBrowserComponent()
		{
			_searchResults = new Dictionary<string, SearchResult>();
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		internal SearchPanelComponent SearchPanelComponent
		{
			get { return _searchPanelComponent; }
			set { _searchPanelComponent = value; }
		}

		public TableData<StudyItem> StudyList
		{
			get { return _currentStudyList; }
			set { _currentStudyList = value; }
		}

		public ActionModelRoot ToolbarModel
		{
			get { return _toolbarModel; }
		}

		public ActionModelRoot ContextMenuModel
		{
			get { return _contextMenuModel; }
		}

		public string ResultsTitle
		{
			get { return _resultsTitle; }
			set
			{
				_resultsTitle = value;
				OnPropertyChanged("ResultsTitle");
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
		}

		public override void Stop()
		{
			base.Stop();
		}

		#endregion

		public void SelectServer(AEServer selectedServer)
		{
			if (_selectedServer == selectedServer)
				return;

			_selectedServer = selectedServer;

			if (selectedServer.Host == "localhost")
				_studyFinder = ImageViewerComponent.StudyManager.StudyFinders["DICOM_LOCAL"];
			else
				_studyFinder = ImageViewerComponent.StudyManager.StudyFinders["DICOM_REMOTE"];

			if (!_searchResults.ContainsKey(_selectedServer.Servername))
			{
				SearchResult searchResult = new SearchResult();
				searchResult.ResultsTitle = String.Format("{0}", _selectedServer.Servername);
				AddColumns(searchResult.StudyList);

				_searchResults.Add(_selectedServer.Servername, searchResult);
			}

			UpdateView();

			EventsHelper.Fire(_selectedServerChangedEvent, this, EventArgs.Empty);
		}

		public void Search()
		{
			Platform.CheckMemberIsSet(_studyFinder, "StudyFinder");
			Platform.CheckMemberIsSet(_searchPanelComponent, "SearchPanelComponent");

			string patientsName = _searchPanelComponent.LastName + 
				GetWildcard() + 
				"^" +
				_searchPanelComponent.FirstName + 
				GetWildcard();
			//string patientsName = _lastName + GetWildcard();
			string patientID = _searchPanelComponent.PatientID + GetWildcard();
			string accessionNumber = _searchPanelComponent.AccessionNumber + GetWildcard();
			string studyDescription = _searchPanelComponent.StudyDescription + GetWildcard();

			QueryParameters queryParams = new QueryParameters();
			queryParams.Add("PatientsName", patientsName);
			queryParams.Add("PatientId", patientID);
			queryParams.Add("AccessionNumber", accessionNumber);
			queryParams.Add("StudyDescription", studyDescription);

			StudyItemList studyItemList;

			try
			{
				studyItemList = _studyFinder.Query(_selectedServer, queryParams);
			}
			catch (Exception e)
			{
				Platform.Log(e, LogLevel.Error);
				throw;
			}

			this.ResultsTitle = String.Format("{0} studies found on {1}", studyItemList.Count, _selectedServer.Servername);
			UpdateComponent();

			foreach (StudyItem item in studyItemList)
				_searchResults[_selectedServer.Servername].StudyList.Add(item);
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

		private void UpdateComponent()
		{
			_searchResults[_selectedServer.Servername].ResultsTitle = this.ResultsTitle;
			_searchResults[_selectedServer.Servername].StudyList.Clear();
		}

		private void UpdateView()
		{
			this.ResultsTitle = _searchResults[_selectedServer.Servername].ResultsTitle;
			this.StudyList = _searchResults[_selectedServer.Servername].StudyList;
		}

		private void AddColumns(TableData<StudyItem> studyList)
		{
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Patient ID",
					delegate(StudyItem item) { return item.PatientId; }
					));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Last Name",
					delegate(StudyItem item) { return item.LastName; }
					));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"First Name",
					delegate(StudyItem item) { return item.FirstName; }
					));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Accession Number",
					delegate(StudyItem item) { return item.AccessionNumber; }
					));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Study Date",
					delegate(StudyItem item) { return item.StudyDate; }
					));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"DOB",
					delegate(StudyItem item) { return item.PatientsBirthDate; }
					));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Description",
					delegate(StudyItem item) { return item.StudyDescription; }
					));
		}

		private string GetWildcard()
		{
			// TODO: get rid of this hack once the dicom datastore layer supports
			// "*" as the proper wildcard character.
			if (_selectedServer.Host == "localhost")
				return "%";
			else
				return "*";
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(
				  this,
				  new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
