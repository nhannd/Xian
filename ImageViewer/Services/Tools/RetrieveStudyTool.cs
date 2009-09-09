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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using System.ServiceModel;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarRetrieveStudy", "RetrieveStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuRetrieveStudy", "RetrieveStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRetrieveStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png")]

	[ViewerActionPermission("activate", ImageViewer.Services.AuthorityTokens.Study.Retrieve)]
	
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class RetrieveStudyTool : StudyBrowserTool
	{
		public RetrieveStudyTool()
		{

		}

		public override void Initialize()
		{
			SetDoubleClickHandler();

			base.Initialize();
		}

		private void RetrieveStudy()
		{
			if (!Enabled || Context.SelectedServerGroup.IsLocalDatastore || Context.SelectedStudy == null)
                return;

			EventResult result = EventResult.Success;

			Dictionary<ApplicationEntity, List<StudyInformation>> retrieveInformation = new Dictionary<ApplicationEntity, List<StudyInformation>>();
			foreach (StudyItem item in Context.SelectedStudies)
			{
				ApplicationEntity applicationEntity = item.Server as ApplicationEntity;
				if (applicationEntity != null && !retrieveInformation.ContainsKey(applicationEntity))
					retrieveInformation[applicationEntity] = new List<StudyInformation>();

				StudyInformation studyInformation = new StudyInformation();
				studyInformation.PatientId = item.PatientId;
				studyInformation.PatientsName = item.PatientsName;
				DateTime studyDate;
				DateParser.Parse(item.StudyDate, out studyDate);
				studyInformation.StudyDate = studyDate;
				studyInformation.StudyDescription = item.StudyDescription;
				studyInformation.StudyInstanceUid = item.StudyInstanceUid;

				retrieveInformation[applicationEntity].Add(studyInformation);
			}

			DicomServerServiceClient client = new DicomServerServiceClient();

			try
			{
				client.Open();

				foreach (KeyValuePair<ApplicationEntity, List<StudyInformation>> kvp in retrieveInformation)
				{
					AEInformation aeInformation = new AEInformation();
					aeInformation.AETitle = kvp.Key.AETitle;
					aeInformation.HostName = kvp.Key.Host;
					aeInformation.Port = kvp.Key.Port;

					client.RetrieveStudies(aeInformation, kvp.Value);
				}

				client.Close();

				LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(Context.DesktopWindow);
			}
			catch (EndpointNotFoundException)
			{
				client.Abort();
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
				foreach (KeyValuePair<ApplicationEntity, List<StudyInformation>> kvp in retrieveInformation)
				{
					AuditedInstances requestedInstances = new AuditedInstances();
					foreach (StudyInformation study in kvp.Value)
						requestedInstances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);
					AuditHelper.LogBeginReceiveInstances(kvp.Key.AETitle, kvp.Key.Host, requestedInstances, EventSource.CurrentUser, result);
				}
			}
		}

		private void SetDoubleClickHandler()
		{
			if (!Context.SelectedServerGroup.IsLocalDatastore && Context.SelectedServerGroup.IsOnlyNonStreamingServers())
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
			                !Context.SelectedServerGroup.IsLocalDatastore &&
			                LocalDataStoreActivityMonitor.IsConnected);
		}
	}
}
