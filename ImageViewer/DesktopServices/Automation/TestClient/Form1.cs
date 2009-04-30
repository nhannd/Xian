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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text;

#if USE_ASP
using ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.ViewerAutomationAsp;
using ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.StudyLocatorAsp;
using ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomationAsp;
using DicomExplorerAutomationClient = ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomationAsp.DicomExplorerAutomation;
using AutomationClient = ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.ViewerAutomationAsp.ViewerAutomation;
using QueryClient = ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.StudyLocatorAsp.StudyLocator;
#else
using ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.ViewerAutomation;
using ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.StudyLocator;
using ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation;
using DicomExplorerAutomationClient = ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.DicomExplorerAutomationClient;
using AutomationClient = ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.ViewerAutomation.ViewerAutomationClient;
using QueryClient = ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.StudyLocator.StudyRootQueryClient;
#endif

namespace ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
#if USE_ASP
			this.Text = "Automation Test Client (ASP)";
#else
			this.Text = "Automation Test Client (WCF)";
#endif
			_studyGrid.SelectionChanged += new EventHandler(OnStudySelectionChanged);

			_dicomExplorerRemoteAE.DataBindings.Add("Enabled", _dicomExplorerQueryRemote, "Enabled", true,
			                                        DataSourceUpdateMode.OnPropertyChanged);
		}

		private void OnClear(object sender, EventArgs e)
		{
			_patientId.Text = "";
			_accession.Text = "";
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

			using (AutomationClient client = new AutomationClient())
			{
				try
				{
					GetViewerInfoRequest request = new GetViewerInfoRequest();
					request.Viewer = new Viewer();
					request.Viewer.Identifier = GetIdentifier(selectedViewer.Value);
					GetViewerInfoResult result = client.GetViewerInfo(request);

					StringBuilder builder = new StringBuilder();
					builder.AppendLine("Additional studies:");

					foreach (string additionalStudyUid in result.AdditionalStudyInstanceUids)
						builder.AppendLine(additionalStudyUid);

					MessageBox.Show(builder.ToString());
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
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

			using (AutomationClient client = new AutomationClient())
			{
				try
				{
					OpenStudiesRequest request = new OpenStudiesRequest();
					BindingList<OpenStudyInfo> studiesToOpen = new BindingList<OpenStudyInfo>();
					foreach (StudyItem s in GetSelectedStudies())
					{
						OpenStudyInfo info = new OpenStudyInfo();
						info.StudyInstanceUid = s.StudyInstanceUid;
						info.SourceAETitle = s.RetrieveAETitle;
						studiesToOpen.Add(info);
					}

					request.StudiesToOpen = GetStudiesToOpen(studiesToOpen);
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
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
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

			using (AutomationClient client = new AutomationClient())
			{
				try
				{
					ActivateViewerRequest request = new ActivateViewerRequest();
					request.Viewer = new Viewer();
					request.Viewer.Identifier = GetIdentifier(viewerId.Value);
					client.ActivateViewer(request);
				}
				catch (Exception ex)
				{
					study.RemoveViewer(viewerId.Value);
					MessageBox.Show(ex.Message);
				}
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

			using (AutomationClient client = new AutomationClient())
			{
				try
				{
					CloseViewerRequest request = new CloseViewerRequest();
					request.Viewer = new Viewer();
					request.Viewer.Identifier = GetIdentifier(viewerId.Value);
					client.CloseViewer(request);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
				finally
				{
					study.RemoveViewer(viewerId.Value);
				}
			}
		}

		#region Dicom Explorer

		private void OnDicomExplorerExecuteQuery(object sender, EventArgs e)
		{
			using (DicomExplorerAutomationClient client = new DicomExplorerAutomationClient())
			{
				try
				{
					if (_dicomExplorerQueryLocal.Checked)
					{
						SearchLocalStudiesRequest request = new SearchLocalStudiesRequest();
						request.SearchCriteria = new DicomExplorerSearchCriteria();
						request.SearchCriteria.PatientId = _patientId.Text;
						request.SearchCriteria.AccessionNumber = _accession.Text;
						client.SearchLocalStudies(request);
					}
					else
					{
						SearchRemoteStudiesRequest request = new SearchRemoteStudiesRequest();
						request.SearchCriteria = new DicomExplorerSearchCriteria();
						request.SearchCriteria.PatientId = _patientId.Text;
						request.SearchCriteria.AccessionNumber = _accession.Text;
						request.AETitle = _dicomExplorerRemoteAE.Text;
						client.SearchRemoteStudies(request);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		#endregion

		private void RefreshStudyList()
		{
			_studyItemBindingSource.Clear();

			using (QueryClient client = new QueryClient())
			{
				try
				{
					StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier();
					string accessionFilter = _accession.Text ?? "";
					if (!String.IsNullOrEmpty(accessionFilter))
						identifier.AccessionNumber = accessionFilter + "*";

					string patientIdFilter = _patientId.Text ?? "";
					if (!String.IsNullOrEmpty(patientIdFilter))
						identifier.PatientId = patientIdFilter + "*";

					BindingList<StudyRootStudyIdentifier> results = DoStudyQuery(client, identifier);

					foreach (StudyRootStudyIdentifier study in results)
						_studyItemBindingSource.Add(new StudyItem(study));
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, ex.Message);
				}
			}
		}

		private void RefreshViewers(bool silent)
		{
			using (AutomationClient client = new AutomationClient())
			{
				try
				{
					GetActiveViewersResult result = client.GetActiveViewers();

					ClearAllViewers();

					foreach (Viewer viewer in result.ActiveViewers)
					{
						StudyItem study = GetStudy(viewer.PrimaryStudyInstanceUid);
						if (study != null)
							study.AddViewer(viewer.Identifier);
					}
				}
				catch (Exception ex)
				{
					ClearAllViewers();
					if (!silent)
						MessageBox.Show(ex.Message);
				}
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

#if USE_ASP

		private static string GetIdentifier(Guid selectedViewer)
		{
			return selectedViewer.ToString();
		}

		private static BindingList<StudyRootStudyIdentifier> DoStudyQuery(QueryClient client, StudyRootStudyIdentifier identifier)
		{
			return new BindingList<StudyRootStudyIdentifier>(client.StudyQuery(identifier));
		}

		private OpenStudyInfo[] GetStudiesToOpen(BindingList<OpenStudyInfo> studiesToOpen)
		{
			OpenStudyInfo[] returnInfo = new OpenStudyInfo[studiesToOpen.Count];
			int i = 0;
			foreach (OpenStudyInfo info in studiesToOpen)
				returnInfo[i++] = info;

			return returnInfo;
		}
#else
		private static Guid GetIdentifier(Guid selectedViewer)
		{
			return selectedViewer;
		}

		private static BindingList<StudyRootStudyIdentifier> DoStudyQuery(QueryClient client, StudyRootStudyIdentifier identifier)
		{
			return client.StudyQuery(identifier);
		}

		private BindingList<OpenStudyInfo> GetStudiesToOpen(BindingList<OpenStudyInfo> studiesToOpen)
		{
			return studiesToOpen;
		}

#endif
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

		public string AccessionNumber
		{
			get { return _study.AccessionNumber; }
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

		public bool HasViewer(string id)
		{
			return _activeViewers.Contains(new Guid(id));
		}

		public bool HasViewer(Guid id)
		{
			return _activeViewers.Contains(id);
		}

		public void AddViewer(string id)
		{
			AddViewer(new Guid(id));
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