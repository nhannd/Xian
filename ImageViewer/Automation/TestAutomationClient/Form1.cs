using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TestAutomationClient.ViewerAutomation;
using ClearCanvas.Dicom.DataStore;
using System.ServiceModel;

namespace TestAutomationClient
{
	public partial class Form1 : Form
	{
		private static readonly string _viewerProcessWorkingDirectory = @"..\..\..\..\..\Desktop\Executable\bin\Debug";
		private static readonly string _viewerProcessExecutable = "ClearCanvas.Desktop.Executable";

		private readonly Dictionary<string, List<Guid>> _activeSessions;

		public Form1()
		{
			InitializeComponent();

			_activeSessions = new Dictionary<string, List<Guid>>();
			_studyGrid.SelectionChanged += new EventHandler(OnStudySelectionChanged);
		}

		void OnStudySelectionChanged(object sender, EventArgs e)
		{
			UpdateActiveSessions();
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

		private void OnOpenStudy(object sender, EventArgs e)
		{
			if (_studyGrid.SelectedRows.Count == 0)
				return;
			
			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();

				string studyUid = GetSelectedStudyInstanceUid();
				List<Guid> existingSessions = GetSessions(studyUid);

				OpenStudiesRequest request = new OpenStudiesRequest();
				request.StudyInstanceUids = new BindingList<string>(GetSelectedStudyInstanceUids());
				request.ActivateIfAlreadyOpen = _activateIfOpen.Checked;
				
				OpenStudiesResult result = client.OpenStudies(request);
				if (result.ViewerSession != null)
				{
					bool shouldMatch = existingSessions.Count > 0 && _activateIfOpen.Checked;
					if (shouldMatch && !existingSessions.Contains(result.ViewerSession.SessionId))
						RemoveAllSessions();
					
					AddSession(result.ViewerSession.SessionId);
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
			if (_studyGrid.SelectedRows.Count != 1)
			{
				MessageBox.Show("Select a single study item in the list.");
				return;
			}

			if (_openSessions.SelectedIndex < 0)
			{
				MessageBox.Show("An active session must be selected.");
				return;
			}

			Guid sessionId = (Guid)_openSessions.SelectedItem;

			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();
				ActivateViewerSessionRequest request = new ActivateViewerSessionRequest();
				request.ViewerSession = new ViewerSession();
				request.ViewerSession.SessionId = sessionId;
				client.ActivateViewerSession(request);
				client.Close();
			}
			catch(FaultException<ViewerSessionNotFoundFault> ex)
			{
				RemoveSession(sessionId);
				client.Abort();
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				RemoveSession(sessionId);
				client.Abort();
				MessageBox.Show(ex.Message);
			}
		}

		private void OnCloseViewer(object sender, EventArgs e)
		{
			if (_studyGrid.SelectedRows.Count != 1)
			{
				MessageBox.Show("Select a single study item in the list.");
				return;
			}

			if (_openSessions.SelectedIndex < 0)
			{
				MessageBox.Show("An active session must be selected.");
				return;
			}

			Guid sessionId = (Guid)_openSessions.SelectedItem;

			ViewerAutomationClient client = new ViewerAutomationClient();
			try
			{
				client.Open();
				CloseViewerSessionRequest request = new CloseViewerSessionRequest();
				request.ViewerSession = new ViewerSession();
				request.ViewerSession.SessionId = sessionId;
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
				RemoveSession(sessionId);
			}
		}

		private string GetSelectedStudyInstanceUid()
		{
			foreach (DataGridViewRow row in _studyGrid.SelectedRows)
				return (string) row.Cells[3].Value;

			return null;
		}

		private IList<string> GetSelectedStudyInstanceUids()
		{
			List<string> studyUids = new List<string>();
			foreach (DataGridViewRow row in _studyGrid.SelectedRows)
				studyUids.Add((string)row.Cells[3].Value);

			return studyUids;
		}

		private void AddSession(Guid sessionId)
		{
			string studyInstanceUid = GetSelectedStudyInstanceUid();
			if (studyInstanceUid == null)
				return;

			if (!_activeSessions.ContainsKey(studyInstanceUid))
				_activeSessions[studyInstanceUid] = new List<Guid>();

			_activeSessions[studyInstanceUid].Add(sessionId);

			UpdateActiveSessions();
		}

		private void RemoveAllSessions()
		{
			string studyInstanceUid = GetSelectedStudyInstanceUid();
			if (studyInstanceUid == null)
				return;

			List<Guid> sessions = GetSessions(studyInstanceUid);
			sessions.Clear();
			
			UpdateActiveSessions();
		}

		private void RemoveSession(Guid sessionId)
		{
			string studyUid = GetSelectedStudyInstanceUid();
			if (studyUid == null)
				return;

			List<Guid> sessions = GetSessions(studyUid);
			sessions.Remove(sessionId);
			
			UpdateActiveSessions();
		}

		private List<Guid> GetSessions(string studyInstanceUid)
		{
			if (studyInstanceUid != null && _activeSessions.ContainsKey(studyInstanceUid))
				return _activeSessions[studyInstanceUid];

			return new List<Guid>();
		}

		private void UpdateActiveSessions()
		{
			string studyInstanceUid = GetSelectedStudyInstanceUid();
			_openSessions.Items.Clear();
			
			if (studyInstanceUid == null)
				return;

			bool anySessions = false;
			List<Guid> sessions = GetSessions(studyInstanceUid);
			if (sessions.Count > 0)
			{
				anySessions = true;
				object[] values = new object[sessions.Count];
				for (int i = 0; i < values.Length; ++i)
					values[i] = sessions[i];

				_openSessions.Items.AddRange(values);
				_openSessions.SelectedIndex = 0;
			}

			_studyGrid.SelectedRows[0].Cells[4].Value = anySessions;
		}

		private void FormLoad(object sender, EventArgs e)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				foreach (IStudy study in reader.GetStudies())
					_studyGrid.Rows.Add(CreateRowValues(study));
			}
		}

		private static object[] CreateRowValues(IStudy study)
		{
			return new object[] { study.PatientsName.ToString(), study.PatientId, study.StudyDescription, study.StudyInstanceUid, false };
		}
	}
}
