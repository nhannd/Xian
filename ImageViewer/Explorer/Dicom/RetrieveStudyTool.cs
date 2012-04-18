#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarRetrieveStudy", "RetrieveStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuRetrieveStudy", "RetrieveStudy")]
    [ActionFormerly("activate", "ClearCanvas.ImageViewer.Services.Tools.RetrieveStudyTool:activate")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRetrieveStudy")]
	[IconSet("activate", "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png")]

    [ViewerActionPermission("activate", ImageViewer.Common.AuthorityTokens.Study.Retrieve)]

	//TODO (Marmot):Restore.
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class RetrieveStudyTool : StudyBrowserTool
	{
		public override void Initialize()
		{
			base.Initialize();

			SetDoubleClickHandler();
		}

		private void RetrieveStudy()
		{
            throw new NotImplementedException("Marmot - need to restore this.");
            /*

            if (!Enabled || Context.SelectedServerGroup.IsLocalDatastore || Context.SelectedStudy == null)
                return;

            EventResult result = EventResult.Success;

            var retrieveInformation = new Dictionary<IApplicationEntity, List<StudyInformation>>();
            foreach (StudyItem item in Context.SelectedStudies)
            {
                var applicationEntity = item.Server as IApplicationEntity;
                if (applicationEntity != null && applicationEntity.ScpParameters != null && !retrieveInformation.ContainsKey(applicationEntity))
                    retrieveInformation[applicationEntity] = new List<StudyInformation>();
                else continue;

                var studyInformation = new StudyInformation {PatientId = item.PatientId, PatientsName = item.PatientsName};
                DateTime studyDate;
                DateParser.Parse(item.StudyDate, out studyDate);
                studyInformation.StudyDate = studyDate;
                studyInformation.StudyDescription = item.StudyDescription;
                studyInformation.StudyInstanceUid = item.StudyInstanceUid;

                retrieveInformation[applicationEntity].Add(studyInformation);
            }

            try
            {
                Platform.GetService(delegate(IDicomServer service)
                                        {
                                            foreach (KeyValuePair<IApplicationEntity, List<StudyInformation>> kvp in retrieveInformation)
                                            {
                                                var aeInformation = new ApplicationEntity
                                                                        {
                                                                            AETitle = kvp.Key.AETitle,
                                                                            ScpParameters = new ScpParameters(kvp.Key.ScpParameters)
                                                                        };

                                                service.RetrieveStudies(aeInformation, kvp.Value);
                                            }
                                        });

                //TODO (Marmot): Restore - just tell the user it's been scheduled.
                //LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(Context.DesktopWindow);
            }
            catch (EndpointNotFoundException)
            {
                result = EventResult.MajorFailure;
                Context.DesktopWindow.ShowMessageBox(SR.MessageRetrieveDicomServerServiceNotRunning, MessageBoxActions.Ok);
            }
            catch (Exception e)
            {
                result = EventResult.MajorFailure;
                ExceptionHandler.Report(e, SR.MessageFailedToRetrieveStudy, Context.DesktopWindow);
            }
            finally
            {
                foreach (KeyValuePair<IApplicationEntity, List<StudyInformation>> kvp in retrieveInformation)
                {
                    var requestedInstances = new AuditedInstances();
                    foreach (StudyInformation study in kvp.Value)
                        requestedInstances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);
                    AuditHelper.LogBeginReceiveInstances(kvp.Key.AETitle, kvp.Key.ScpParameters.HostName, requestedInstances, EventSource.CurrentUser, result);
                }
            }
             */
		}

		private bool GetAtLeastOneServerSupportsLoading()
		{
			if (Context.SelectedServerGroup.IsLocalServer && base.IsLocalStudyLoaderSupported)
				return true;

			foreach (IServerTreeDicomServer server in base.Context.SelectedServerGroup.Servers)
			{
				if (server.IsStreaming && base.IsStreamingStudyLoaderSupported)
					return true;
				else if (!server.IsStreaming && base.IsRemoteStudyLoaderSupported)
					return true;
			}

			return false;
		}

		private void SetDoubleClickHandler()
		{
			if (!GetAtLeastOneServerSupportsLoading() && base.Context.SelectedServerGroup.Servers.Count > 0)
				Context.DefaultActionHandler = RetrieveStudy;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}
		
		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
			SetDoubleClickHandler();
		}

		private void UpdateEnabled()
		{
			Enabled = (Context.SelectedStudy != null &&
			                !Context.SelectedServerGroup.IsLocalServer &&
			                WorkItemActivityMonitor.IsRunning);
		}
	}
}