using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.IO;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using System.ServiceModel;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarRetrieveStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuRetrieveStudy")]
	[ClickHandler("activate", "RetrieveStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRetrieveStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.RetrieveStudySmall.png", "Icons.RetrieveStudySmall.png", "Icons.RetrieveStudySmall.png")]
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
			if (this.Context.SelectedServerGroup.IsLocalDatastore)
				return;

            if (this.Context.SelectedStudy == null)
                return;

			Dictionary<ApplicationEntity, List<StudyInformation>> retrieveInformation = new Dictionary<ApplicationEntity, List<StudyInformation>>();
			foreach (StudyItem item in this.Context.SelectedStudies)
			{
				if (!retrieveInformation.ContainsKey(item.Server))
					retrieveInformation[item.Server] = new List<StudyInformation>();

				StudyInformation studyInformation = new StudyInformation();
				studyInformation.PatientId = item.PatientId;
				studyInformation.PatientsName = item.PatientsName;
				DateTime studyDate;
				DateParser.Parse(item.StudyDate, out studyDate);
				studyInformation.StudyDate = studyDate;
				studyInformation.StudyDescription = item.StudyDescription;
				studyInformation.StudyInstanceUid = item.StudyInstanceUID;

				retrieveInformation[item.Server].Add(studyInformation);
			}

			DicomServerServiceClient client = new DicomServerServiceClient();

			try
			{
				client.Open();

				foreach (KeyValuePair<ApplicationEntity, List<StudyInformation>> kvp in retrieveInformation)
				{
					AEInformation aeInformation = new AEInformation();
					aeInformation.AETitle = kvp.Key.AE;
					aeInformation.HostName = kvp.Key.Host;
					aeInformation.Port = kvp.Key.Port;

					client.RetrieveStudies(aeInformation, kvp.Value);
				}
								
				client.Close();

				LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(this.Context.DesktopWindow);
			}
			catch (EndpointNotFoundException)
			{
				client.Abort();
				Platform.ShowMessageBox(SR.MessageRetrieveDicomServerServiceNotRunning);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToRetrieveStudy, this.Context.DesktopWindow);
			}
		}

		private void SetDoubleClickHandler()
		{
			if (!this.Context.SelectedServerGroup.IsLocalDatastore)
				this.Context.DefaultActionHandler = RetrieveStudy;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			// If the results aren't from a remote machine, then we don't
			// even care whether a study has been selected or not
			if (this.Context.SelectedServerGroup.IsLocalDatastore)
				return;

			base.OnSelectedStudyChanged(sender, e);
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			// If no study is selected then we don't even care whether
			// the last searched server has changed.

			if (this.Context.SelectedServerGroup.IsLocalDatastore)
			{
				this.Enabled = false;
				return;
			}
			else
			{
				if (this.Context.SelectedStudy != null)
					this.Enabled = true;
				else
					this.Enabled = false;

				SetDoubleClickHandler();
			}
        }
    }
}
