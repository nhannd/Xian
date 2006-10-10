using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom;
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

		AEServerGroup SelectedServerGroup { get; }

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

		private IStudyFinder _studyFinder;
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

		public Table<StudyItem> StudyList
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
			_toolSet.Dispose();
			_toolSet = null;

			base.Stop();
		}

		#endregion

		public void SelectServerGroup(AEServerGroup selectedServerGroup)
		{
			//if (_selectedServerGroup != null)
			//{
			//    if (_selectedServerGroup.GroupID == selectedServerGroup.GroupID)
			//        return;
			//}

			_selectedServerGroup = selectedServerGroup;

			if (selectedServerGroup.IsLocalDatastore)
				_studyFinder = ImageViewerComponent.StudyManager.StudyFinders["DICOM_LOCAL"];
			else
				_studyFinder = ImageViewerComponent.StudyManager.StudyFinders["DICOM_REMOTE"];

			if (!_searchResults.ContainsKey(_selectedServerGroup.GroupID))
			{
				SearchResult searchResult = new SearchResult();
				searchResult.ResultsTitle = String.Format("{0}", _selectedServerGroup.Name);
				AddColumns(searchResult.StudyList);

				_searchResults.Add(_selectedServerGroup.GroupID, searchResult);
			}

			UpdateView();

			EventsHelper.Fire(_selectedServerChangedEvent, this, EventArgs.Empty);
		}

		public void Search()
		{
            InternalSearch(DateTime.MinValue, DateTime.MinValue);
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
			_searchResults[_selectedServerGroup.GroupID].ResultsTitle = this.ResultsTitle;
			_searchResults[_selectedServerGroup.GroupID].StudyList.Items.Clear();
		}

		private void UpdateView()
		{
			this.ResultsTitle = _searchResults[_selectedServerGroup.GroupID].ResultsTitle;
			this.StudyList = _searchResults[_selectedServerGroup.GroupID].StudyList;
		}

		private void AddColumns(Table<StudyItem> studyList)
		{
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Patient ID",
					delegate(StudyItem item) { return item.PatientId; },
                    1.5f
					));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Last Name",
					delegate(StudyItem item) { return item.LastName; },
                    1.5f
					));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"First Name",
					delegate(StudyItem item) { return item.FirstName; }
					));
            studyList.Columns.Add(
                new TableColumn<StudyItem, string>(
                    "DOB",
                    delegate(StudyItem item) { return DicomHelper.ConvertFromDicomDA(item.PatientsBirthDate); },
                    null,
                    1.0f,
                    delegate(StudyItem one, StudyItem two) { return one.PatientsBirthDate.CompareTo(two.PatientsBirthDate); }
                    ));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Accession Number",
					delegate(StudyItem item) { return item.AccessionNumber; }
					));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Study Date",
					delegate(StudyItem item) { return DicomHelper.ConvertFromDicomDA(item.StudyDate); },
                    null,
                    1.0f,
                    delegate(StudyItem one, StudyItem two) {  return one.StudyDate.CompareTo(two.StudyDate); }
					));
			studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Description",
					delegate(StudyItem item) { return item.StudyDescription; },
                    2.5f
					));
            studyList.Columns.Add(
                new TableColumn<StudyItem, string>(
                    "Modality",
                    delegate(StudyItem item) { return item.ModalitiesInStudy; },
                    0.5f
                    ));

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

        internal void SearchToday()
        {
            InternalSearch(DateTime.Today, DateTime.Today);
        }

        /// <summary>
        /// The semantics of the fromDate and toDate, is:
        /// <table>
        /// <tr><td>fromDate</td><td>toDate</td><td>Query</td></tr>
        /// <tr><td>null</td><td>null</td><td>Empty</td></tr>
        /// <tr><td>20060608</td><td>null</td><td>Since: "20060608-"</td></tr>
        /// <tr><td>20060608</td><td>20060610</td><td>Between: "20060608-20060610"</td></tr>
        /// <tr><td>null</td><td>20060610</td><td>Prior to: "-20060610"</td></tr>
        /// </table>
        /// Treat "null" above as DateTime.MinValue
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        private void InternalSearch(DateTime fromDate, DateTime toDate)
        {
            Platform.CheckMemberIsSet(_studyFinder, "StudyFinder");
            Platform.CheckMemberIsSet(_searchPanelComponent, "SearchPanelComponent");

            // create patient's name query key
            // LastName   FirstName   Result
            //    X           X        <Blank>
            //    V           X        LastName*
            //    V           V        LastName*FirstName*
            //    X           V        *FirstName*
            string patientsName = "";
            if (_searchPanelComponent.LastName.Length > 0 && _searchPanelComponent.FirstName.Length == 0)
                patientsName = _searchPanelComponent.LastName + "*";
            if (_searchPanelComponent.LastName.Length > 0 && _searchPanelComponent.FirstName.Length > 0)
                patientsName = _searchPanelComponent.LastName + "*" + _searchPanelComponent.FirstName + "*";
            if (_searchPanelComponent.LastName.Length == 0 && _searchPanelComponent.FirstName.Length > 0)
                patientsName = "*" + _searchPanelComponent.FirstName + "*";

            string patientId = "";
            if (_searchPanelComponent.PatientID.Length > 0)
                patientId = _searchPanelComponent.PatientID + "*";

            string accessionNumber = "";
            if (_searchPanelComponent.AccessionNumber.Length > 0)
                accessionNumber = _searchPanelComponent.AccessionNumber + "*";

            string studyDescription = "";
            if (_searchPanelComponent.StudyDescription.Length > 0)
                studyDescription = _searchPanelComponent.StudyDescription + "*";

            QueryParameters queryParams = new QueryParameters();
            queryParams.Add("PatientsName", patientsName);
            queryParams.Add("PatientId", patientId);
            queryParams.Add("AccessionNumber", accessionNumber);
            queryParams.Add("StudyDescription", studyDescription);
            queryParams.Add("ModalitiesInStudy", "");
            queryParams.Add("StudyDate", GetDicomDateRangeMatchString(fromDate, toDate));

            StudyItemList aggregateStudyItemList = new StudyItemList();

            try
            {
                foreach (AEServer server in _selectedServerGroup.Servers)
                {
                    StudyItemList serverStudyItemList = _studyFinder.Query(server, queryParams);
                    aggregateStudyItemList.AddRange(serverStudyItemList);
                }
            }
            catch (Exception e)
            {
                Platform.Log(e, LogLevel.Error);
                throw;
            }
            finally
            {
                this.ResultsTitle = String.Format("{0} studies found on {1}", aggregateStudyItemList.Count, _selectedServerGroup.Name);

                UpdateComponent();

                foreach (StudyItem item in aggregateStudyItemList)
                    _searchResults[_selectedServerGroup.GroupID].StudyList.Items.Add(item);

                _searchResults[_selectedServerGroup.GroupID].StudyList.Sort();
            }
        }

        private string GetDicomDateRangeMatchString(DateTime fromDate, DateTime toDate)
        {
            if (DateTime.MinValue == fromDate && DateTime.MinValue == toDate)
            {
                return "";
            }
            else if (fromDate == toDate)
            {
                return fromDate.ToString("yyyyMMdd");
            }
            else if (DateTime.MinValue != fromDate && DateTime.MinValue == toDate)
            {
                return fromDate.ToString("yyyyMMdd-");
            }
            else if (DateTime.MinValue != fromDate && DateTime.MinValue != toDate)
            {
                return fromDate.ToString("yyyyMMdd-") + toDate.ToString("yyyyMMdd");
            }
            else if (DateTime.MinValue == fromDate && DateTime.MinValue != toDate)
            {
                return toDate.ToString("-yyyyMMdd");
            }

            return "";
        }

    }
}
