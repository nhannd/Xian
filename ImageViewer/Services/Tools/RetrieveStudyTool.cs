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

			Dictionary<ApplicationEntity, List<string>> retrieveInformation = new Dictionary<ApplicationEntity, List<string>>();
			foreach (StudyItem item in this.Context.SelectedStudies)
			{
				if (!retrieveInformation.ContainsKey(item.Server))
					retrieveInformation[item.Server] = new List<string>();

				retrieveInformation[item.Server].Add(item.StudyInstanceUID);
			}

			DicomServerServiceClient client = new DicomServerServiceClient();

			try
			{
				client.Open();

				foreach (KeyValuePair<ApplicationEntity, List<string>> kvp in retrieveInformation)
				{
					DicomRetrieveRequest request = new DicomRetrieveRequest();
					request.RetrieveLevel = RetrieveLevel.Study;

					request.SourceAETitle = kvp.Key.AE;
					request.SourceHostName = kvp.Key.Host;
					request.Port = kvp.Key.Port;
					request.Uids = kvp.Value;

					client.Retrieve(request);
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
