using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;


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

		public void RetrieveStudy()
		{
			if (this.Context.SelectedStudy == null)
				return;

			ApplicationEntity me = new ApplicationEntity(new HostName("localhost"), new AETitle("CCWORKSTN"), new ListeningPort(4000));
			using (DicomClient client = new DicomClient(me))
			{
				AEServer server = this.Context.LastSearchedServer;
				Uid studyUid = new Uid(this.Context.SelectedStudy.StudyInstanceUID);
				client.Retrieve(server, studyUid, "c:\\TestImages");
			}
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			// If the results aren't from a remote machine, then we don't
			// even care whether a study has been selected or not
			if (this.Context.LastSearchedServer.Host == "localhost")
				return;

			base.OnSelectedStudyChanged(sender, e);
		}

		protected override void OnLastSearchedServerChanged(object sender, EventArgs e)
		{
			// If no study is selected then we don't even care whether
			// the last searched server has changed.
			if (this.Context.SelectedStudy == null)
				return;

			if (this.Context.LastSearchedServer.Host == "localhost")
				this.Enabled = false;
			else
				this.Enabled = true;
		}
	}
}
