#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Utilities.Anonymization;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using Path=System.IO.Path;

namespace ClearCanvas.Utilities.DicomEditor
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarAnonymizeStudy", "AnonymizeStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuAnonymizeStudy", "AnonymizeStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipAnonymizeStudy")]
	[IconSet("activate", "Icons.AnonymizeToolSmall.png", "Icons.AnonymizeToolSmall.png", "Icons.AnonymizeToolSmall.png")]

	[ViewerActionPermission("activate", AuthorityTokens.Study.Anonymize)]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class AnonymizeStudyTool : StudyBrowserTool
	{
		private volatile AnonymizeStudyComponent _component;
		private string _tempPath;
		
		public void AnonymizeStudy()
		{
			_component = new AnonymizeStudyComponent(Context.SelectedStudy);
			if (ApplicationComponentExitCode.Accepted == 
				ApplicationComponent.LaunchAsDialog(Context.DesktopWindow, _component, SR.TitleAnonymizeStudy))
			{
				BackgroundTask task = null;
				try
				{
					task = new BackgroundTask(Anonymize, false, Context.SelectedStudy);
					ProgressDialog.Show(task, Context.DesktopWindow, true);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					string message = String.Format(SR.MessageFormatStudyMustBeDeletedManually, _tempPath);
					Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
				}
				finally
				{
					_tempPath = null;

					if (task != null)
						task.Dispose();
				}
			}
		}

        private static DicomServerConfiguration GetServerConfiguration()
        {
            DicomServerConfiguration configuration = null;
            var request = new GetDicomServerConfigurationRequest();
            Platform.GetService<IDicomServerConfiguration>(s =>
                                                            configuration =
                                                            s.GetConfiguration(request).Configuration);

            return configuration;
        }

        private void Anonymize(IBackgroundTaskContext context)
        {
            //TODO (Marmot) This probably should be its own WorkItem type and have it done in the background there.
            var study = (StudyTableItem) context.UserState;
            var anonymizedInstances = new AuditedInstances();

            try
            {
                _tempPath = Path.Combine(Path.GetTempPath(), "ClearCanvas");
                _tempPath = Path.Combine(_tempPath, "Anonymization");
                _tempPath = Path.Combine(_tempPath, Path.GetRandomFileName());
                Directory.CreateDirectory(_tempPath);

                context.ReportProgress(new BackgroundTaskProgress(0, SR.MessageAnonymizingStudy));

                var loader = study.Server.GetService<IStudyLoader>();
                int numberOfSops = loader.Start(new StudyLoaderArgs(study.StudyInstanceUid, null));
                if (numberOfSops <= 0)
                    return;

                var anonymizer = new DicomAnonymizer {StudyDataPrototype = _component.AnonymizedData};

                if (_component.PreserveSeriesData)
                {
                    //The default anonymizer removes the series data, so we just clone the original.
                    anonymizer.AnonymizeSeriesDataDelegate = original => original.Clone();
                }

                // Setup the ImportFilesUtility to perform the import
                var configuration = GetServerConfiguration();
                var importContext = new ImportStudyContext(configuration.AETitle, StudyStore.GetConfiguration());
                var importUtility = new ImportFilesUtility(importContext);

                // setup auditing information
                var result = EventResult.Success;

                string patientsSex = null;

                for (int i = 0; i < numberOfSops; ++i)
                {
                    using (var sop = loader.LoadNextSop())
                    {
                        if (sop != null &&
                            (_component.KeepReportsAndAttachments || !IsReportOrAttachmentSopClass(sop.SopClassUid)))
                        {
                            //preserve the patient sex.
                            if (patientsSex == null)
                                anonymizer.StudyDataPrototype.PatientsSex = patientsSex = sop.PatientsSex ?? "";

                            var localSopDataSource = sop.DataSource as ILocalSopDataSource;
                            if (localSopDataSource != null)
                            {
                                string filename = Path.Combine(_tempPath, string.Format("{0}.dcm", i));
                                DicomFile file = (localSopDataSource).File;

                                // make sure we anonymize a new instance, not the same instance that the Sop cache holds!!
                                file = new DicomFile(filename, file.MetaInfo.Copy(), file.DataSet.Copy());
                                anonymizer.Anonymize(file);

                                // Do the import
                                var importResult = importUtility.Import(file, BadFileBehaviourEnum.Ignore,
                                                                        FileImportBehaviourEnum.Save);
                                if (importResult.DicomStatus != DicomStatuses.Success)
                                {
                                    Platform.Log(LogLevel.Warn, "Unable to import published file: {0}",
                                                 importResult.ErrorMessage);
                                }

                                string studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
                                string patientId = file.DataSet[DicomTags.PatientId].ToString();
                                string patientsName = file.DataSet[DicomTags.PatientsName].ToString();
                                anonymizedInstances.AddInstance(patientId, patientsName, studyInstanceUid);
                            }
                        }
                    }

                    int progressPercent = (int) Math.Floor((i + 1)/(float) numberOfSops*100);
                    string progressMessage = String.Format(SR.MessageAnonymizingStudy, _tempPath);
                    context.ReportProgress(new BackgroundTaskProgress(progressPercent, progressMessage));
                }

                AuditHelper.LogCreateInstances(new[]{configuration.AETitle}, anonymizedInstances, EventSource.CurrentUser, result);

                context.Complete();
            }
            catch (Exception e)
            {
                AuditHelper.LogCreateInstances(new[] { string.Empty }, anonymizedInstances, EventSource.CurrentUser, EventResult.MajorFailure);
                context.Error(e);
            }
        }

	    private void UpdateEnabled()
		{
		    Enabled = Context.SelectedStudies.Count == 1
		              && Context.SelectedServers.AllSupport<IWorkItemService>()
		              && WorkItemActivityMonitor.IsRunning;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		internal static bool IsReportOrAttachmentSopClass(string sopClassUid)
		{
			return sopClassUid == SopClass.EncapsulatedPdfStorageUid
			       || sopClassUid == SopClass.EncapsulatedCdaStorageUid
			       || sopClassUid == SopClass.BasicTextSrStorageUid
			       || sopClassUid == SopClass.ChestCadSrStorageUid
			       || sopClassUid == SopClass.ColonCadSrStorageUid
			       || sopClassUid == SopClass.ComprehensiveSrStorageTrialRetiredUid
			       || sopClassUid == SopClass.ComprehensiveSrStorageUid
			       || sopClassUid == SopClass.EnhancedSrStorageUid
			       || sopClassUid == SopClass.MammographyCadSrStorageUid
			       || sopClassUid == SopClass.TextSrStorageTrialRetiredUid
			       || sopClassUid == SopClass.XRayRadiationDoseSrStorageUid;
		}
	}
}
