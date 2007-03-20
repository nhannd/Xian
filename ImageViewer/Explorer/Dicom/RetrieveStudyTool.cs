using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using System.IO;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/Retrieve")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/Retrieve")]
	[ClickHandler("activate", "RetrieveStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "Retrieve Study")]
	[IconSet("activate", IconScheme.Colour, "Icons.SendStudySmall.png", "Icons.SendStudySmall.png", "Icons.SendStudySmall.png")]
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
            //
            // check pre-conditions
            //

			if (this.Context.SelectedServerGroup.IsLocalDatastore)
				return;

            if (this.Context.SelectedStudy == null)
                return;

			DicomRetrieveRequest request = new DicomRetrieveRequest();
			request.SourceAETitle = this.Context.SelectedStudy.Server.AE;
			request.SourceHostName = this.Context.SelectedStudy.Server.Host; ;
			request.Port = this.Context.SelectedStudy.Server.Port;

			List<string> studyUids = new List<string>();
			foreach (StudyItem item in this.Context.SelectedStudies)
				studyUids.Add(item.StudyInstanceUID);

			request.Uids = studyUids.ToArray();

			DicomServerServiceClient client = new DicomServerServiceClient();

			try
			{
				client.Open();
				client.Retrieve(request);
				client.Close();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
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
