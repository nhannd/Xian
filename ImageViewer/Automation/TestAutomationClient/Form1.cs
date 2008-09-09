using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TestAutomationClient.ViewerAutomation;
using ClearCanvas.Dicom.DataStore;
using System.ServiceModel;
using System.Text;

namespace TestAutomationClient
{
	public partial class Form1 : Form
	{
		private static readonly string _viewerProcessWorkingDirectory = @"..\..\..\..\..\Desktop\Executable\bin\Debug";
		private static readonly string _viewerProcessExecutable = "ClearCanvas.Desktop.Executable";

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
				_openSessions.DataSource = null;
			}
			else
			{
				_openSessions.DataSource = study.ActiveSessions;
				if (study.HasSessions)
					_openSessions.SelectedIndex = 0;
			}
		}

		private void StartViewer(object sender, EventArgs e)
		{
			string workingDirectory = Directory.GetCurrentDirectory();

			Process[] viewerProcesses = Process.GetProcessesByName(_viewerProcessExecutable);
			if (viewerProcesses == null || viewerProcesses.Length == 0)
			{
				string viewerProcessPath = String.Format("{0}\\{1}\\{2}.exe", workingDirectory, _viewerProcessWorkingDirectory, _viewerProcessExecutable);

				ProcessStartInfo startInfo = new ProcessStartInfo(viewerProcessPath, "");
				startInfo.WorkingDirectory = _viewerProcessWorkingDirectory;
				Process viewerProcess = Process.Start(startInfo);
				if (viewerProcess == null)
					MessageBox.Show("Failed to start the viewer process.");
			}
		}

		private void GetSelectedInfo(object sender, EventArgs e)
		{
			StudyItem study = GetSelectedStudy();
			if (study == null)
			{
				MessageBox.Show("Select a single study item in the list.");
				return;
			}

			Guid? selectedSession = GetSelectedSession();
			if (selectedSession == null)
			{
				MessageBox.Show("An active session must be selected.");
				return;
			}

			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();
				GetViewerSessionInfoRequest request = new GetViewerSessionInfoRequest();
				request.ViewerSession = new ViewerSession();
				request.ViewerSession.SessionId = selectedSession.Value;
				GetViewerSessionInfoResult result = client.GetViewerSessionInfo(request);
				client.Close();

				StringBuilder builder = new StringBuilder();
				builder.AppendLine("Additional studies:");

				foreach (string additionalStudyUid in result.AdditionalStudyInstanceUids)
					builder.AppendLine(additionalStudyUid);

				MessageBox.Show(builder.ToString());
			}
			catch (FaultException<NoActiveViewerSessionsFault> ex)
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

		private void OnRefreshAll(object sender, EventArgs e)
		{
			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();
				GetActiveViewerSessionsResult result = client.GetActiveViewerSessions();

				ClearAllSessions();
				
				foreach (ViewerSession session in result.ActiveViewerSessions)
				{
					StudyItem study = GetStudy(session.PrimaryStudyInstanceUid);
					if (study != null)
						study.AddSession(session.SessionId);
				}

		
				client.Close();
			}
			catch (FaultException<NoActiveViewerSessionsFault> ex)
			{
				ClearAllSessions();
				client.Abort();
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				ClearAllSessions();
				client.Abort();
				MessageBox.Show(ex.Message);
			}
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
				List<string> uids = new List<string>(GetSelectedStudyInstanceUids());
				request.StudyInstanceUids = new BindingList<string>(uids);
				request.ActivateIfAlreadyOpen = _activateIfOpen.Checked;
				
				OpenStudiesResult result = client.OpenStudies(request);
				if (result.ViewerSession != null)
				{
					bool shouldExist = study.HasSessions && _activateIfOpen.Checked;
					bool exists = study.HasSession(result.ViewerSession.SessionId);
					if (shouldExist && !exists)
						study.ClearSessions();
				
					if (!exists)
						study.AddSession(result.ViewerSession.SessionId);
				}

				client.Close();
			}
			catch(FaultException<OpenStudiesFault> ex)
			{
				client.Abort();
				MessageBox.Show(ex.Message);
			}
			catch(Exception ex)
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

			Guid? sessionId = GetSelectedSession();
			if (sessionId == null)
			{
				MessageBox.Show("An active session must be selected.");
				return;
			}

			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();
				ActivateViewerSessionRequest request = new ActivateViewerSessionRequest();
				request.ViewerSession = new ViewerSession();
				request.ViewerSession.SessionId = sessionId.Value;
				client.ActivateViewerSession(request);
				client.Close();
			}
			catch(FaultException<ViewerSessionNotFoundFault> ex)
			{
				study.RemoveSession(sessionId.Value);
				client.Abort();
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				study.RemoveSession(sessionId.Value);
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

			Guid? sessionId = GetSelectedSession();
			if (sessionId == null)
			{
				MessageBox.Show("An active session must be selected.");
				return;
			}

			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();
				CloseViewerSessionRequest request = new CloseViewerSessionRequest();
				request.ViewerSession = new ViewerSession();
				request.ViewerSession.SessionId = sessionId.Value;
				client.CloseViewerSession(request);
				client.Close();
			}
			catch (FaultException<ViewerSessionNotFoundFault> ex)
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
				study.RemoveSession(sessionId.Value);
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

		private List<string> GetSelectedStudyInstanceUids()
		{
			List<string> studyUids = new List<string>();
			foreach (StudyItem study in GetSelectedStudies())
				studyUids.Add(study.StudyInstanceUid);
			
			return studyUids;
		}

		private List<StudyItem> GetSelectedStudies()
		{
			List<StudyItem> studies = new List<StudyItem>();
			foreach (DataGridViewRow row in _studyGrid.SelectedRows)
				studies.Add((StudyItem)row.DataBoundItem);

			studies.Reverse();

			return studies;
		}

		private Guid? GetSelectedSession()
		{
			if (_openSessions.SelectedIndex < 0)
				return null;

			return (Guid)_openSessions.SelectedItem;
		}

		private void ClearAllSessions()
		{
			foreach (DataGridViewRow row in _studyGrid.Rows)
			{
				StudyItem item = (StudyItem)row.DataBoundItem;
				item.ClearSessions();
			}
		}

		private void FormLoad(object sender, EventArgs e)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				foreach (IStudy study in reader.GetStudies())
					_studyItemBindingSource.Add(new StudyItem(study));
			}
		}
	}

	#region StudyItem class

	public class StudyItem : INotifyPropertyChanged
	{
		private readonly IStudy _study;
		private readonly BindingList<Guid> _activeSessions;

		public StudyItem(IStudy study)
		{
			_study = study;
			_activeSessions = new BindingList<Guid>();
		}

		public string PatientId
		{
			get { return _study.PatientId; }
		}

		public string PatientsName
		{
			get { return _study.PatientsName.ToString(); }
		}

		public string StudyDescription
		{
			get { return _study.StudyDescription; }
		}

		public string StudyInstanceUid
		{
			get { return _study.StudyInstanceUid; }
		}

		public bool HasSessions
		{
			get { return _activeSessions.Count > 0; }
		}

		internal IEnumerable<Guid> ActiveSessions
		{
			get { return _activeSessions; }
		}

		public bool HasSession(Guid guid)
		{
			return _activeSessions.Contains(guid);
		}

		public void AddSession(Guid session)
		{
			if (!_activeSessions.Contains(session))
				_activeSessions.Add(session);

			FirePropertyChanged("HasSessions");
		}

		public void RemoveSession(Guid session)
		{
			_activeSessions.Remove(session);
			FirePropertyChanged("HasSessions");
		}

		public void ClearSessions()
		{
			_activeSessions.Clear();
			FirePropertyChanged("HasSessions");
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
