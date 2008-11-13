#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Anonymization;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Common.Utilities;
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
			_component = new AnonymizeStudyComponent(this.Context.SelectedStudy);
			if (ApplicationComponentExitCode.Accepted == 
				ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, _component, SR.TitleAnonymizeStudy))
			{
				BackgroundTask task = null;
				try
				{
					task = new BackgroundTask(Anonymize, false, this.Context.SelectedStudy.StudyInstanceUID);
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
			try
			{
				_tempPath = String.Format(".\\temp\\{0}", Path.GetRandomFileName());
				Directory.CreateDirectory(_tempPath);
				_tempPath = Path.GetFullPath(_tempPath);

				context.ReportProgress(new BackgroundTaskProgress(0, SR.MessageAnonymizingStudy));

				int numberOfSops = LocalStudyLoader.Start(new StudyLoaderArgs(this.Context.SelectedStudy.StudyInstanceUID, null));
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

				for (int i = 0; i < numberOfSops; ++i)
				{
					Sop sop = LocalStudyLoader.LoadNextImage();
					//preserve the patient sex.
					if (patientsSex == null)
						anonymizer.StudyDataPrototype.PatientsSex = patientsSex = sop.PatientsSex ?? "";

					DicomFile file = (DicomFile)sop.NativeDicomObject;

					anonymizer.Anonymize(file);

					file.Save(String.Format("{0}\\{1}.dcm", _tempPath, i));

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
					List<string> filePaths = new List<string>();
					filePaths.Add(_tempPath);
					request.FilePaths = filePaths;
					request.Recursive = true;
					client.Import(request);
					client.Close();
				}
				catch
				{
					client.Abort();
					throw;
				}

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
							LocalDataStoreActivityMonitor.Instance.IsConnected && 
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
