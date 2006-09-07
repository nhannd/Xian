using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("send", "dicomstudybrowser-toolbar/Send")]
	[MenuAction("send", "dicomstudybrowser-contextmenu/Send")]
	[ClickHandler("send", "SendStudy")]
	[EnabledStateObserver("send", "Enabled", "EnabledChanged")]
	[Tooltip("send", "Send Study")]
	[IconSet("send", IconScheme.Colour, "Icons.SendStudySmall.png", "Icons.SendStudySmall.png", "Icons.SendStudySmall.png")]
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
	}
}
