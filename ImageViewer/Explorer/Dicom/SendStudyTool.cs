using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/Send")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/Send")]
	[ClickHandler("activate", "SendStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "Send Study")]
	[IconSet("activate", IconScheme.Colour, "Icons.SendStudySmall.png", "Icons.SendStudySmall.png", "Icons.SendStudySmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class SendStudyTool : StudyBrowserTool
	{
		public SendStudyTool()
		{

		}

		public void SendStudy()
		{
			Platform.ShowMessageBox("Not yet implemented!");
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			if (this.Context.LastSearchedServer.Host == "localhost")
				base.OnSelectedStudyChanged(sender, e);
		}

		protected override void OnLastSearchedServerChanged(object sender, EventArgs e)
		{
			if (this.Context.LastSearchedServer.Host == "localhost")
				this.Enabled = true;
			else
				this.Enabled = false;
		}
	}
}
