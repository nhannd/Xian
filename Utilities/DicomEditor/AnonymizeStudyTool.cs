#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Anonymization;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.StudyManagement;
using Path=System.IO.Path;

namespace ClearCanvas.Utilities.DicomEditor
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarAnonymizeStudy", "AnonymizeStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuAnonymizeStudy", "AnonymizeStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipAnonymizeStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.AnonymizeToolSmall.png", "Icons.AnonymizeToolSmall.png", "Icons.AnonymizeToolSmall.png")]

	[ViewerActionPermission("activate", AuthorityTokens.Study.Anonymize)]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class AnonymizeStudyTool : StudyBrowserTool
	{
		private volatile AnonymizeStudyComponent _component;
		private string _tempPath;
		private static object _localStudyLoader = null;
		
		public AnonymizeStudyTool()
		{
		}

		private static IStudyLoader LocalStudyLoader
		{
			get
			{
				if (_localStudyLoader == null)
				{
					try
					{
						StudyLoaderExtensionPoint xp = new StudyLoaderExtensionPoint();
						foreach (IStudyLoader loader in xp.CreateExtensions())
						{
							if (loader.Name == "DICOM_LOCAL")
							{
								_localStudyLoader = loader;
								break;
							}
						}
					}
					catch (NotSupportedException)
					{
						Platform.Log(LogLevel.Info, "Anonymization tool disabled; no local study loader exists.");
					}

					if (_localStudyLoader == null)
						_localStudyLoader = new object(); //there is no loader.
				}

				return _localStudyLoader as IStudyLoader;
			}
		}

		public void AnonymizeStudy()
		{
			StudyItem selectedStudy = this.Context.SelectedStudy;

			_component = new AnonymizeStudyComponent(this.Context.SelectedStudy);
			if (ApplicationComponentExitCode.Accepted == 
				ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, _component, SR.TitleAnonymizeStudy))
			{
				BackgroundTask task = null;
				try
				{
					task = new BackgroundTask(Anonymize, false, this.Context.SelectedStudy);
					ProgressDialog.Show(task, this.Context.DesktopWindow, true);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					string message = String.Format(SR.MessageFormatStudyMustBeDeletedManually, _tempPath);
					this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
				}
				finally
				{
					_tempPath = null;

					if (task != null)
						task.Dispose();
				}
			}
		}

		private void Anonymize(IBackgroundTaskContext context)
		{
			StudyItem study = (StudyItem)context.UserState;
			AuditedInstances anonymizedInstances = new AuditedInstances();

			try
			{
				_tempPath = Path.Combine(Path.GetTempPath(), "ClearCanvas");
				_tempPath = Path.Combine(_tempPath, "Anonymization");
				_tempPath = Path.Combine(_tempPath, Path.GetRandomFileName());
				Directory.CreateDirectory(_tempPath);

				context.ReportProgress(new BackgroundTaskProgress(0, SR.MessageAnonymizingStudy));

				int numberOfSops = LocalStudyLoader.Start(new StudyLoaderArgs(study.StudyInstanceUid, null));
				if (numberOfSops <= 0)
					return;

				DicomAnonymizer anonymizer = new DicomAnonymizer();
				anonymizer.StudyDataPrototype = _component.AnonymizedData;

				if (_component.PreserveSeriesData)
				{
					//The default anonymizer removes the series data, so we just clone the original.
					anonymizer.AnonymizeSeriesDataDelegate = 
						delegate(SeriesData original) { return original.Clone(); };
				}

				string patientsSex = null;
				List<string> filePaths = new List<string>();

				for (int i = 0; i < numberOfSops; ++i)
				{
					Sop sop = LocalStudyLoader.LoadNextSop();
					if (sop != null)
					{
						//preserve the patient sex.
						if (patientsSex == null)
							anonymizer.StudyDataPrototype.PatientsSex = patientsSex = sop.PatientsSex ?? "";

						if (sop.DataSource is ILocalSopDataSource)
						{
							string filename = Path.Combine(_tempPath, string.Format("{0}.dcm", i));
							DicomFile file = ((ILocalSopDataSource)sop.DataSource).File;

							// make sure we anonymize a new instance, not the same instance that the Sop cache holds!!
							file = new DicomFile(filename, file.MetaInfo.Copy(), file.DataSet.Copy());
							anonymizer.Anonymize(file);
							filePaths.Add(filename);
							file.Save(filename);

							string studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
							string patientId = file.DataSet[DicomTags.PatientId].ToString();
							string patientsName = file.DataSet[DicomTags.PatientsName].ToString();
							anonymizedInstances.AddInstance(patientId, patientsName, studyInstanceUid);
						}
					}

					int progressPercent = (int)Math.Floor((i + 1) / (float)numberOfSops * 100);
					string progressMessage = String.Format(SR.MessageAnonymizingStudy, _tempPath);
					context.ReportProgress(new BackgroundTaskProgress(progressPercent, progressMessage));
				}

				//trigger an import of the anonymized files.
				LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
				client.Open();
				try
				{
					FileImportRequest request = new FileImportRequest();
					request.BadFileBehaviour = BadFileBehaviour.Move;
					request.FileImportBehaviour = FileImportBehaviour.Move;
					request.FilePaths = new string[] {_tempPath};
					request.Recursive = false;
					request.IsBackground = true;
					client.Import(request);
					client.Close();
				}
				catch
				{
					client.Abort();
					throw;
				}

				AuditHelper.LogCreateInstances(new string[0], anonymizedInstances, EventSource.CurrentUser, EventResult.Success);

				context.Complete();
			}
			catch(Exception e)
			{
				context.Error(e);
			}
		}

		private void UpdateEnabled()
		{
			if (this.Context.SelectedStudy == null)
			{
				this.Enabled = false;
				return;
			}

			this.Enabled = LocalStudyLoader != null && 
							LocalDataStoreActivityMonitor.IsConnected && 
							this.Context.SelectedStudies.Count == 1 && 
							this.Context.SelectedServerGroup.IsLocalDatastore;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}
	}
}
