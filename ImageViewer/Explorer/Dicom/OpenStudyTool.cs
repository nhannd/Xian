using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("open", "dicomstudybrowser-toolbar/Open")]
	[MenuAction("open", "dicomstudybrowser-contextmenu/Open")]
	[ClickHandler("open", "OpenStudy")]
	[EnabledStateObserver("open", "Enabled", "EnabledChanged")]
	[Tooltip("open", "Open Study")]
	[IconSet("open", IconScheme.Colour, "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class OpenStudyTool : StudyBrowserTool
	{
		public OpenStudyTool()
		{

		}

		public override void Initialize()
		{
			this.Context.DefaultActionHandler = OpenStudy;
			base.Initialize();
		}

		public void OpenStudy()
		{
			this.Context.StudyBrowserComponent.Open();
		}
	}
}
