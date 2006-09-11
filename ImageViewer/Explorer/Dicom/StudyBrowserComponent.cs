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

		AEServer LastSearchedServer { get; }

		event EventHandler SelectedStudyChanged;

		event EventHandler SelectedServerChanged;

		event EventHandler LastSearchedServerChanged;

		IStudyLoader StudyLoader { get; }

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

			public AEServer LastSearchedServer
			{
				get { return _component._lastSearchedServer; }
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

			public event EventHandler LastSearchedServerChanged
			{
				add { _component.LastSearchedServerChanged += value; }
				remove { _component.LastSearchedServerChanged -= value; }
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

			public IStudyLoader StudyLoader
			{
				get { return _component._studyLoader; }
			}

			public void RefreshStudyList()
			{
				_component.Search();
			}

			#endregion
		}
	
		#region Fields

		private IStudyFinder _studyFinder;
		private IStudyLoader _studyLoader;
		private TableData<StudyItem> _studyList;

		private string _title;

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

		private AEServer _lastSearchedServer;
		private event EventHandler _lastSearchedServerChangedEvent;

		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#endregion

		public TableData<StudyItem> StudyList
		{
			get { return _studyList; }
		}

		public ActionModelRoot ToolbarModel
		{
			get { return _toolbarModel; }
		}

		public ActionModelRoot ContextMenuModel
		{
			get { return _contextMenuModel; }
		}
	

		public string Title
		{
			get { return _title; }
			set 
			{ 
				_title = value;
				OnPropertyChanged("Title");
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
			get { return _currentSelection.Item as StudyItem; }
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

		private event EventHandler LastSearchedServerChanged
		{
			add { _lastSearchedServerChangedEvent += value; }
			remove { _lastSearchedServerChangedEvent -= value; }
		}

		#region IApplicationComponent overrides

		public override void Start()
		{
			base.Start();

			_studyList = new TableData<StudyItem>();

			AddColumns();

			_toolSet = new ToolSet(new StudyBrowserToolExtensionPoint(), new StudyBrowserToolContext(this));
			_toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomstudybrowser-toolbar", _toolSet.Actions);
			_contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomstudybrowser-contextmenu", _toolSet.Actions);
			_studyLoader = ImageViewerComponent.StudyManager.StudyLoaders["DICOM_LOCAL"];
			_title = "Search";
		}

		public override void Stop()
		{
			base.Stop();
		}

		#endregion

		public void Search()
		{
			Platform.CheckMemberIsSet(_studyFinder, "StudyFinder");

			string patientsName = _lastName + GetWildcard() + "^" + _firstName + GetWildcard();
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

			_studyList.Clear();

			foreach (StudyItem item in studyItemList)
				_studyList.Add(item);

			this.Title = String.Format("Search Results from {0}", _selectedServer.Servername);

			if (_selectedServer != _lastSearchedServer)
			{
				_lastSearchedServer = _selectedServer;
				EventsHelper.Fire(_lastSearchedServerChangedEvent, this, EventArgs.Empty);
			}
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

		public void SelectServer(AEServer selectedServer)
		{
			_selectedServer = selectedServer;

			if (_lastSearchedServer == null)
				_lastSearchedServer = selectedServer;

			if (selectedServer.Host == "localhost")
				_studyFinder = ImageViewerComponent.StudyManager.StudyFinders["DICOM_LOCAL"];
			else
				_studyFinder = ImageViewerComponent.StudyManager.StudyFinders["DICOM_REMOTE"];

			EventsHelper.Fire(_selectedServerChangedEvent, this, EventArgs.Empty);
		}

		private void AddColumns()
		{
			_studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Patient ID",
					delegate(StudyItem item) { return item.PatientId; }
					));
			_studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Last Name",
					delegate(StudyItem item) { return item.LastName; }
					));
			_studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"First Name",
					delegate(StudyItem item) { return item.FirstName; }
					));
			_studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Accession Number",
					delegate(StudyItem item) { return item.AccessionNumber; }
					));
			_studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"DOB",
					delegate(StudyItem item) { return item.PatientsBirthDate; }
					));
			_studyList.Columns.Add(
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
