using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.ServiceModel;
using System.Text;
using ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.ViewerAutomation;
using ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.StudyLocator;

namespace ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			_studyGrid.SelectionChanged += new EventHandler(OnStudySelectionChanged);
		}

		private void OnStudySelectionChanged(object sender, EventArgs e)
		{
			StudyItem study = GetSelectedStudy();
			if (study == null)
			{
				_openViewers.DataSource = null;
			}
			else
			{
				_openViewers.DataSource = study.ActiveViewers;
				if (study.HasViewers)
					_openViewers.SelectedIndex = 0;
			}
		}

		private void OnStartViewer(object sender, EventArgs e)
		{
			Process[] viewerProcesses = Process.GetProcessesByName(Settings.Default.ViewerProcessExecutable);
			if (viewerProcesses == null || viewerProcesses.Length == 0)
			{
				string viewerProcessPath = Settings.Default.ViewerWorkingDirectory;
				if (!Path.IsPathRooted(viewerProcessPath))
					viewerProcessPath = Path.Combine(Directory.GetCurrentDirectory(), viewerProcessPath);

				string executable = Path.Combine(viewerProcessPath, Settings.Default.ViewerProcessExecutable);
				executable += ".exe";

				ProcessStartInfo startInfo = new ProcessStartInfo(executable, "");
				startInfo.WorkingDirectory = viewerProcessPath;
				Process viewerProcess = Process.Start(startInfo);
				if (viewerProcess == null)
					MessageBox.Show("Failed to start the viewer process.");
			}
		}

		private void OnGetSelectedInfo(object sender, EventArgs e)
		{
			StudyItem study = GetSelectedStudy();
			if (study == null)
			{
				MessageBox.Show("Select a single study item in the list.");
				return;
			}

			Guid? selectedViewer = GetSelectedViewer();
			if (selectedViewer == null)
			{
				MessageBox.Show("An active viewer must be selected.");
				return;
			}

			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();
				GetViewerInfoRequest request = new GetViewerInfoRequest();
				request.Viewer = new Viewer();
				request.Viewer.Identifier = selectedViewer.Value;
				GetViewerInfoResult result = client.GetViewerInfo(request);
				client.Close();

				StringBuilder builder = new StringBuilder();
				builder.AppendLine("Additional studies:");

				foreach (string additionalStudyUid in result.AdditionalStudyInstanceUids)
					builder.AppendLine(additionalStudyUid);

				MessageBox.Show(builder.ToString());
			}
			catch (FaultException<NoActiveViewersFault> ex)
			{
				client.Abort();
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				client.Abort();
				MessageBox.Show(ex.Message);
			}
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			RefreshStudyList();
		}

		private void OnRequery(object sender, EventArgs e)
		{
			RefreshStudyList();
			RefreshViewers(true);
		}

		private void OnRefreshAllViewers(object sender, EventArgs e)
		{
			RefreshViewers(false);
		}

		private void OnOpenStudy(object sender, EventArgs e)
		{
			StudyItem study = GetSelectedStudy();
			if (study == null)
				return;

			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();

				OpenStudiesRequest request = new OpenStudiesRequest();
				BindingList<OpenStudyInfo> studiesToOpen = new BindingList<OpenStudyInfo>();
				foreach (StudyItem s in GetSelectedStudies())
				{
					OpenStudyInfo info = new OpenStudyInfo();
					info.StudyInstanceUid = s.StudyInstanceUid;
					//info.SourceAETitle = s.RetrieveAETitle;
					studiesToOpen.Add(info);
				}

				request.StudiesToOpen = studiesToOpen;
				request.ActivateIfAlreadyOpen = _activateIfOpen.Checked;

				OpenStudiesResult result = client.OpenStudies(request);
				if (result.Viewer != null)
				{
					bool shouldExist = study.HasViewers && _activateIfOpen.Checked;
					bool exists = study.HasViewer(result.Viewer.Identifier);
					if (shouldExist && !exists)
						study.ClearViewers();

					if (!exists)
						study.AddViewer(result.Viewer.Identifier);
				}

				client.Close();
			}
			catch (FaultException<OpenStudiesFault> ex)
			{
				client.Abort();
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				client.Abort();
				MessageBox.Show(ex.Message);
			}
		}

		private void OnActivateViewer(object sender, EventArgs e)
		{
			StudyItem study = GetSelectedStudy();
			if (study == null)
			{
				MessageBox.Show("Select a single study item in the list.");
				return;
			}

			Guid? viewerId = GetSelectedViewer();
			if (viewerId == null)
			{
				MessageBox.Show("An active viewer must be selected.");
				return;
			}

			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();
				ActivateViewerRequest request = new ActivateViewerRequest();
				request.Viewer = new Viewer();
				request.Viewer.Identifier = viewerId.Value;
				client.ActivateViewer(request);
				client.Close();
			}
			catch(FaultException<ViewerNotFoundFault> ex)
			{
				study.RemoveViewer(viewerId.Value);
				client.Abort();
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				study.RemoveViewer(viewerId.Value);
				client.Abort();
				MessageBox.Show(ex.Message);
			}
		}

		private void OnCloseViewer(object sender, EventArgs e)
		{
			StudyItem study = GetSelectedStudy();
			if (study == null)
			{
				MessageBox.Show("Select a single study item in the list.");
				return;
			}

			Guid? viewerId = GetSelectedViewer();
			if (viewerId == null)
			{
				MessageBox.Show("An active viewer must be selected.");
				return;
			}

			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();
				CloseViewerRequest request = new CloseViewerRequest();
				request.Viewer = new Viewer();
				request.Viewer.Identifier = viewerId.Value;
				client.CloseViewer(request);
				client.Close();
			}
			catch (FaultException<ViewerNotFoundFault> ex)
			{
				client.Abort();
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				client.Abort();
				MessageBox.Show(ex.Message);
			}
			finally
			{
				study.RemoveViewer(viewerId.Value);
			}
		}

		private void RefreshStudyList()
		{
			_studyItemBindingSource.Clear();

			StudyRootQueryClient client = new StudyRootQueryClient();

			try
			{
				StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier();
				BindingList<StudyRootStudyIdentifier> results = client.StudyQuery(identifier);
				client.Close();

				foreach (StudyRootStudyIdentifier study in results)
					_studyItemBindingSource.Add(new StudyItem(study));
			}
			catch (Exception ex)
			{
				client.Abort();
				MessageBox.Show(this, ex.Message);
			}
		}

		private void RefreshViewers(bool silent)
		{
			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();
				GetActiveViewersResult result = client.GetActiveViewers();

				ClearAllViewers();

				foreach (Viewer viewer in result.ActiveViewers)
				{
					StudyItem study = GetStudy(viewer.PrimaryStudyInstanceUid);
					if (study != null)
						study.AddViewer(viewer.Identifier);
				}

				client.Close();
			}
			catch (FaultException<NoActiveViewersFault> ex)
			{
				ClearAllViewers();
				client.Abort();

				if (!silent)
					MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				ClearAllViewers();
				client.Abort();
				MessageBox.Show(ex.Message);
			}
		}

		private StudyItem GetStudy(string studyInstanceUid)
		{
			foreach (DataGridViewRow row in _studyGrid.Rows)
			{
				if (studyInstanceUid == ((StudyItem)row.DataBoundItem).StudyInstanceUid)
					return row.DataBoundItem as StudyItem;
			}

			return null;
		}

		private StudyItem GetSelectedStudy()
		{
			List<StudyItem> selected = GetSelectedStudies();
			if (selected.Count > 0)
				return selected[0];

			return null;
		}

		private IEnumerable<string> GetSelectedStudyInstanceUids()
		{
			foreach (StudyItem study in GetSelectedStudies())
				yield return study.StudyInstanceUid;
		}

		private List<StudyItem> GetSelectedStudies()
		{
			List<StudyItem> studies = new List<StudyItem>();
			foreach (DataGridViewRow row in _studyGrid.SelectedRows)
				studies.Add((StudyItem)row.DataBoundItem);

			studies.Reverse();

			return studies;
		}

		private Guid? GetSelectedViewer()
		{
			if (_openViewers.SelectedIndex < 0)
				return null;

			return (Guid)_openViewers.SelectedItem;
		}

		private void ClearAllViewers()
		{
			foreach (DataGridViewRow row in _studyGrid.Rows)
			{
				StudyItem item = (StudyItem)row.DataBoundItem;
				item.ClearViewers();
			}
		}
	}

	#region StudyItem class

	public class StudyItem : INotifyPropertyChanged
	{
		private readonly StudyRootStudyIdentifier _study;
		private readonly BindingList<Guid> _activeViewers;

		public StudyItem(StudyRootStudyIdentifier study)
		{
			_study = study;
			_activeViewers = new BindingList<Guid>();
		}

		public string PatientId
		{
			get { return _study.PatientId; }
		}

		public string PatientsName
		{
			get { return _study.PatientsName; }
		}

		public string StudyDescription
		{
			get { return _study.StudyDescription; }
		}

		public string StudyInstanceUid
		{
			get { return _study.StudyInstanceUid; }
		}

		public string RetrieveAETitle
		{
			get { return _study.RetrieveAeTitle; }	
		}

		public bool HasViewers
		{
			get { return _activeViewers.Count > 0; }
		}

		internal IEnumerable<Guid> ActiveViewers
		{
			get { return _activeViewers; }
		}

		public bool HasViewer(Guid id)
		{
			return _activeViewers.Contains(id);
		}

		public void AddViewer(Guid id)
		{
			if (!_activeViewers.Contains(id))
				_activeViewers.Add(id);

			FirePropertyChanged("HasViewers");
		}

		public void RemoveViewer(Guid id)
		{
			_activeViewers.Remove(id);
			FirePropertyChanged("HasViewers");
		}

		public void ClearViewers()
		{
			_activeViewers.Clear();
			FirePropertyChanged("HasViewers");
		}

		private void FirePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
	#endregion
}
