using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint()]
	public class StudyBrowserComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(StudyBrowserComponentViewExtensionPoint))]
	public class StudyBrowserComponent : ApplicationComponent
	{
	
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
		
		#endregion

		public IStudyFinder StudyFinder
		{
			get { return _studyFinder; }
			set { _studyFinder = value; }
		}

		public IStudyLoader StudyLoader
		{
			get { return _studyLoader; }
			set { _studyLoader = value; }
		}

		public TableData<StudyItem> StudyList
		{
			get { return _studyList; }
		}
		
		public string Title
		{
			get { return _title; }
			set { _title = value; }
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
			set { _firstName = value; }
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

		public ISelection CurrentSelection
		{
			get { return _currentSelection;}
			set { _currentSelection = value;}
		}

		#region IApplicationComponent overrides

		public override void Start()
		{
			base.Start();

			_studyList = new TableData<StudyItem>();

			AddColumns();
		}

		public override void Stop()
		{
			base.Stop();
		}

		#endregion

		public void Search()
		{
			Platform.CheckMemberIsSet(_studyFinder, "StudyFinder");

			QueryParameters queryParams = new QueryParameters();
			queryParams.Add("PatientsName",_lastName);
			queryParams.Add("PatientId", _patientID);
			queryParams.Add("AccessionNumber", _accessionNumber);
			queryParams.Add("StudyDescription", _studyDescription);
			
			StudyItemList studyItemList = _studyFinder.Query(queryParams);

			_studyList.Clear();

			foreach (StudyItem item in studyItemList)
				_studyList.Add(item);
		}

		public void Clear()
		{
			this.PatientID = "";
			this.FirstName = "";
			this.LastName = "";
			this.AccessionNumber = "";
			this.StudyDescription = "";
		}

		public void Open()
		{
			StudyItem selectedStudy = _currentSelection.Item as StudyItem;

			if (selectedStudy == null)
				return;

			string studyInstanceUid = selectedStudy.StudyInstanceUID;
			string label = String.Format("{0}, {1} | {2}", 
				selectedStudy.LastName, 
				selectedStudy.FirstName,
				selectedStudy.PatientId);

			_studyLoader.LoadStudy(studyInstanceUid);

			ImageViewerComponent imageViewer = new ImageViewerComponent(studyInstanceUid);
			ApplicationComponent.LaunchAsWorkspace(this.Host.DesktopWindow, imageViewer, label, null);
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
					"DOB",
					delegate(StudyItem item) { return item.PatientsBirthDate; }
					));
			_studyList.Columns.Add(
				new TableColumn<StudyItem, string>(
					"Description",
					delegate(StudyItem item) { return item.StudyDescription; }
					));
		}
	}
}
