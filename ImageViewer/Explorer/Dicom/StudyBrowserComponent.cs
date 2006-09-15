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
				get { return _component.SelectedStudy; }
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
			private string _lastName = "";
			private string _firstName = "";
			private string _patientID = "";
			private string _accessionNumber = "";
			private string _studyDescription = "";

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

			public string AccessionNumber
			{
				get { return _accessionNumber; }
				set { _accessionNumber = value; }
			}

			public string PatientID
			{
				get { return _patientID; }
				set { _patientID = value; }
			}

			public string FirstName
			{
				get { return _firstName; }
				set	{ _firstName = value; }
			}

			public string LastName
			{
				get { return _lastName; }
				set { _lastName = value; }
			}

			public string StudyDescription
			{
				get { return _studyDescription; }
				set { _studyDescription = value; }
			}
		}
	
		#region Fields

		private IStudyFinder _studyFinder;
		private Dictionary<string, SearchResult> _searchResults;
		private TableData<StudyItem> _currentStudyList;

		private string _searchTitle;
		private string _resultsTitle;

		private string _lastName = "";
		private string _firstName = "";
		private string _patientID = "";
		private string _accessionNumber = "";
		private string _studyDescription = "";

		private ISelection _currentSelection;
		private event EventHandler _selectedStudyChangedEvent;
		private ClickHandlerDelegate _defaultActionHandler;
		private ToolSet _toolSet;

		private AEServer _selectedServer;
		private event EventHandler _selectedServerChangedEvent;

		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#endregion

		public StudyBrowserComponent()
		{
			_searchResults = new Dictionary<string, SearchResult>();
		}

		public TableData<StudyItem> StudyList
		{
			get { return _currentStudyList; }
			set
			{
				_currentStudyList = value;
				OnPropertyChanged("StudyList");
			}
		}

		public ActionModelRoot ToolbarModel
		{
			get { return _toolbarModel; }
		}

		public ActionModelRoot ContextMenuModel
		{
			get { return _contextMenuModel; }
		}

		public string SearchTitle
		{
			get { return _searchTitle; }
			set
			{ 
				_searchTitle = value;
				OnPropertyChanged("SearchTitle");
			}
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

		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set 
			{ 
				_accessionNumber = value;
				OnPropertyChanged("AccessionNumber");
			}
		}

		public string PatientID
		{
			get { return _patientID; }
			set 
			{ 
				_patientID = value;
				OnPropertyChanged("PatientID");
			}
		}

		public string FirstName
		{
			get { return _firstName; }
			set 
			{ 
				_firstName = value;
				OnPropertyChanged("FirstName");
			}
		}

		public string LastName
		{
			get { return _lastName; }
			set 
			{ 
				_lastName = value;
				OnPropertyChanged("LastName");
			}
		}

		public string StudyDescription
		{
			get { return _studyDescription; }
			set 
			{ 
				_studyDescription = value;
				OnPropertyChanged("StudyDescription");
			}
		}

		private StudyItem SelectedStudy
		{
			get 
			{
				if (_currentSelection == null)
					return null;

				return _currentSelection.Item as StudyItem; 
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
			this.SearchTitle = "Search";
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

			string patientsName = _lastName + GetWildcard() + "^" + _firstName + GetWildcard();
			//string patientsName = _lastName + GetWildcard();
			string patientID = _patientID + GetWildcard();
			string accessionNumber = _accessionNumber + GetWildcard();
			string studyDescription = _studyDescription + GetWildcard();

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

		public void Clear()
		{
			this.PatientID = "";
			this.FirstName = "";
			this.LastName = "";
			this.AccessionNumber = "";
			this.StudyDescription = "";
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
			_searchResults[_selectedServer.Servername].PatientID = this.PatientID;
			_searchResults[_selectedServer.Servername].LastName = this.LastName;
			_searchResults[_selectedServer.Servername].FirstName = this.FirstName;
			_searchResults[_selectedServer.Servername].AccessionNumber = this.AccessionNumber;
			_searchResults[_selectedServer.Servername].StudyDescription = this.StudyDescription;
			_searchResults[_selectedServer.Servername].StudyList.Clear();
		}

		private void UpdateView()
		{
			this.ResultsTitle = _searchResults[_selectedServer.Servername].ResultsTitle;
			this.StudyList = _searchResults[_selectedServer.Servername].StudyList;

			//this.PatientID = _searchResults[_selectedServer.Servername].PatientID;
			//this.LastName = _searchResults[_selectedServer.Servername].LastName;
			//this.FirstName = _searchResults[_selectedServer.Servername].FirstName;
			//this.AccessionNumber = _searchResults[_selectedServer.Servername].AccessionNumber;
			//this.StudyDescription = _searchResults[_selectedServer.Servername].StudyDescription;
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
			// TODO: get rid of this hack once the dicom layer supports
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
